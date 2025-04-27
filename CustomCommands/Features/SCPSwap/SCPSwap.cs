using CustomCommands.Core;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using PlayerRoles.RoleAssign;
using RedRightHand;
using RoundRestarting;
using System;
using System.Collections.Generic;
using System.Linq;
using UserSettings.ServerSpecific;

namespace CustomCommands.Features.SCPSwap
{
    public class SCPSwap : CustomFeature
    {
        internal enum SwapType
        {
            SwappedToHuman,
            SwappedToSCP,
        }

        public static bool SwapPaused = false;

        static int _swapSeconds = 60;
        public static int SwapToHumanSeconds => _swapSeconds;
        public static int SwapToScpSeconds => (int)(_swapSeconds * 1.5f);

        public static int SCPsToReplace = 0;
        static int ReplaceBaseCooldownRounds = 3;

        static Dictionary<string, int> triggers = new Dictionary<string, int>();
        static Dictionary<string, int> scpCooldown = new Dictionary<string, int>();
        static Dictionary<string, int> humanCooldown = new Dictionary<string, int>();
        static Dictionary<string, string> swapRequests = new Dictionary<string, string>();
        static Dictionary<string, SwapType> swapDict = new Dictionary<string, SwapType>();
        private static readonly RoleTypeId[] scpRoles = { RoleTypeId.Scp049, RoleTypeId.Scp079, RoleTypeId.Scp106, RoleTypeId.Scp173, RoleTypeId.Scp939, RoleTypeId.Scp096 };

        public class Raffle
        {
            /// <summary>
            /// Player ID, Raffle weighting
            /// </summary>
            private Dictionary<int, uint> raffleParticipants = new Dictionary<int, uint>();
            public bool IsActive { get; private set; } = true;

            public Raffle() { IsActive = false; }
            public Raffle(int PlayerID, uint Weighting, float RaffleTime = 5f)
            {
                _ = AddParticipant(PlayerID, Weighting);
                startRaffle(RaffleTime);
            }
            public bool AddParticipant(int PlayerID, uint Weighting)
            {
                if (IsActive && raffleParticipants.ContainsKey(PlayerID)) return false;
                raffleParticipants.Add(PlayerID, Weighting);
                return true;
            }
            private void startRaffle(float time)
            {
                MEC.Timing.CallDelayed(time, () =>
                {
                    bool NwMoment = false;
                    int draw = -1;
                    List<int> winners = new List<int>();
                    if (raffleParticipants.Count == 0) return;
                    List<KeyValuePair<int, uint>> DrawGroup = raffleParticipants.ToList();
                    if (raffleParticipants.Count >= 6)
                    {
                        DrawGroup.Sort((x, y) => x.Value.CompareTo(y.Value));
                        DrawGroup = raffleParticipants.Skip(DrawGroup.Count / 2).ToList();
                    }
                    DrawGroup.ShuffleList();

                redraw: //Makes sure the person didn't leave in the 5 second draw time and that all SCP slots are filled
                    if (DrawGroup.Count == 0)
                    {
                        finishRaffle(winners);
                        return;
                    }

                    draw = DrawGroup.First().Key;
                    DrawGroup.RemoveAt(0);

                retryPlayer:

                    try
                    {
                        if (Player.TryGet(draw, out var drawPlr))
                        {
                            SwapHumanToScp(drawPlr);
                            winners.Add(draw);
                            NwMoment = false;
                        }
                        else goto redraw;
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e.Message);
                        if (NwMoment)
                        {
                            Logger.Info($"Skipping raffle participant with ID {draw}");
                            NwMoment = false;
                            goto redraw;
                        }
                        else
                        {
                            NwMoment = true;
                            goto retryPlayer;
                        }
                    }

                    if (SCPsToReplace == 0)
                        finishRaffle(winners);
                    else goto redraw;
                });
            }
            private void finishRaffle(List<int> winners)
            {
                while (raffleParticipants.Count > 0)
                {
                    try
                    {
                        if (!winners.Contains(raffleParticipants.Last().Key))
                            if (Player.TryGet(raffleParticipants.Last().Key, out var plr))
                                plr.SendHint("You lost the raffle.", 3);
                    }
                    catch (Exception e) { Logger.Error(e.Message); }
                    raffleParticipants.Remove(raffleParticipants.Last().Key);
                }
                IsActive = false;
            }
        }

        public static Raffle raffle;

        public static RoleTypeId[] AvailableSCPs
        {
            get
            {
                var tempRoles = scpRoles.ToList();
                var currentScpRoles = Player.List.Where(r => r.ReferenceHub.IsSCP(false)).Select(r => r.Role);

                if (!currentScpRoles.Any())
                {
                    _ = tempRoles.Remove(RoleTypeId.Scp079);
                    _ = tempRoles.Remove(RoleTypeId.Scp096);
                }

                foreach (var r in currentScpRoles)
                {
                    _ = tempRoles.Remove(r);

                    if (r == RoleTypeId.Scp079 || r == RoleTypeId.Scp096) // 079/096 exclusivity
                    {
                        _ = tempRoles.Remove(RoleTypeId.Scp079);
                        _ = tempRoles.Remove(RoleTypeId.Scp096);
                    }
                }

                if (tempRoles.Any()) // Incase this is used on a really big server
                    return tempRoles.ToArray();
                else
                    return scpRoles;
            }
        }

        public SCPSwap(bool configSetting) : base(configSetting)
        {
        }

        public static void ReplaceBroadcast()
        {
            Server.ClearBroadcasts();
            Server.SendBroadcast($"There {(SCPsToReplace == 1 ? "is" : "are")} now {SCPsToReplace} SCP spot{(SCPsToReplace == 1 ? "" : "s")} available. Run \".scp\" to queue for an SCP", 5);
        }

        public static bool CanScpSwapToHuman(ReferenceHub plr, out string reason) => CanScpSwapToHuman(Player.Get(plr), out reason);
        public static bool CanScpSwapToHuman(Player plr, out string reason)
        {
            if (!CustomCommandsPlugin.Config.EnableScpSwap)
            {
                reason = "Swapping is not enabled on this server";
                return false;
            }

            if (SwapPaused)
            {
                reason = "Swapping has been paused";
                return false;
            }

            if (!plr.IsSCP || plr.Role == RoleTypeId.Scp0492)
            {
                reason = "You must be an SCP to run this command";
                return false;
            }

            if (plr.Health != plr.MaxHealth)
            {
                reason = "You cannot swap as you have taken damage";
                return false;
            }
            if (swapDict.ContainsKey(plr.UserId))
            {
                reason = "You cannot swap back to human";
                return false;
            }
            if (humanCooldown.TryGetValue(plr.UserId, out int roundCount))
            {
                if (roundCount > RoundRestart.UptimeRounds)
                {
                    reason = $"You are on cooldown for another {roundCount - RoundRestart.UptimeRounds} round(s).";
                    return false;
                }
            }
            if (Round.Duration > TimeSpan.FromSeconds(SwapToHumanSeconds))
            {
                reason = $"You can only swap from SCP within the first {SwapToHumanSeconds} seconds of a round";
                return false;
            }

            reason = string.Empty;
            return true;
        }

        public static bool CanHumanSwapToScp(ReferenceHub plr, out string reason) => CanHumanSwapToScp(Player.Get(plr), out reason);
        public static bool CanHumanSwapToScp(Player plr, out string reason)
        {
            if (!CustomCommandsPlugin.Config.EnableScpSwap)
            {
                reason = "Swapping is not enabled on this server";
                return false;
            }

            if (SwapPaused)
            {
                reason = "Swapping has been paused";
                return false;
            }
            if (SCPsToReplace < 1)
            {
                reason = "There are no SCPs to replace";
                return false;
            }
            if (swapDict.ContainsKey(plr.UserId) || plr.IsSCP)
            {
                reason = "You were already an SCP this round";
                return false;
            }
            if (Round.Duration > TimeSpan.FromSeconds(SwapToScpSeconds))
            {
                reason = $"You can only replace an SCP within the first {SwapToScpSeconds} seconds of the round";
                return false;
            }
            if (scpCooldown.TryGetValue(plr.UserId, out int roundCount))
            {
                if (roundCount > RoundRestart.UptimeRounds)
                {
                    if (SCPSwap.triggers.TryGetValue(plr.UserId, out int count))
                    {
                        if (count > 2)
                        {
                            SCPSwap.scpCooldown[plr.UserId]++;
                            SCPSwap.triggers[plr.UserId] = 0;
                        }
                        else SCPSwap.triggers[plr.UserId]++;
                    }
                    else
                        SCPSwap.triggers.Add(plr.UserId, 1);


                    reason = $"You are on cooldown for another {SCPSwap.scpCooldown[plr.UserId] - RoundRestart.UptimeRounds} round(s).";
                    return false;
                }
            }

            reason = string.Empty;
            return true;
        }

        public static void SwapScpToHuman(ReferenceHub plr) => SwapScpToHuman(Player.Get(plr));
        public static void SwapScpToHuman(Player plr)
        {
            SCPSwap.SCPsToReplace++;
            HumanSpawner.SpawnLate(plr.ReferenceHub);
            swapDict.AddToOrReplaceValue(plr.UserId, SwapType.SwappedToHuman);
            SCPSwap.ReplaceBroadcast();

            humanCooldown.AddToOrReplaceValue(plr.UserId, RoundRestart.UptimeRounds + SCPSwap.ReplaceBaseCooldownRounds);
        }

        public static void QueueSwapHumanToScp(ReferenceHub plr) => QueueSwapHumanToScp(Player.Get(plr));
        public static void QueueSwapHumanToScp(Player plr)
        {
            ScpTicketsLoader tix = new ScpTicketsLoader();
            int numTix = tix.GetTickets(plr.ReferenceHub, 10);
            tix.Dispose();
            uint rGroup = 1;
            if (numTix >= 13) rGroup = (uint)numTix;
            if (plr.Role == RoleTypeId.Spectator) rGroup <<= 8;

            if (raffle != null && raffle.IsActive)
            {
                if (raffle.AddParticipant(plr.PlayerId, rGroup))
                    plr.SendHint("You are now in the SCP raffle", 3);
            }
            else
            {
                raffle = new Raffle(plr.PlayerId, rGroup);
                plr.SendHint("You are now in the SCP raffle", 3);
            }
        }

        public static void SwapHumanToScp(ReferenceHub plr) => SwapHumanToScp(Player.Get(plr));
        public static void SwapHumanToScp(Player plr)
        {
            var scps = SCPSwap.AvailableSCPs;

            plr.SetRole(scps[new Random().Next(0, scps.Length)], RoleChangeReason.LateJoin);
            ScpTicketsLoader tix = new ScpTicketsLoader();
            tix.ModifyTickets(plr.ReferenceHub, 10);
            tix.Dispose();
            swapDict.AddToOrReplaceValue(plr.UserId, SwapType.SwappedToSCP);

            SCPSwap.SCPsToReplace--;
            scpCooldown.AddToOrReplaceValue(plr.UserId, RoundRestart.UptimeRounds + SCPSwap.ReplaceBaseCooldownRounds);
        }

        public static bool HasSwapRequest(Player plr, out Player swapper)
        {
            swapper = null;
            return swapRequests.TryGetValue(plr.UserId, out var swapperID) && Player.TryGet(swapperID, out swapper) ||
                swapRequests.Values.Where(r => r == plr.UserId).Any();
        }
        public static void RemoveSwapRequest(Player plr)
        {
            if (swapRequests.ContainsKey(plr.UserId))
                swapRequests.Remove(plr.UserId);
        }
        public static void AddSwapRequest(Player player, Player target)
        {
            //This is reversed so that you can easily check from the target side if they have any pending requests;
            swapRequests.AddToOrReplaceValue(target.UserId, player.UserId);

            target.SendHint($"{player.Nickname} wants to swap SCP with you. Type `.sswapa` in your console to swap to {player.Role}, or type `.sswapd` to reject the request", 8);
            target.SendConsoleMessage($"{player.Nickname} wants to swap SCP with you. Type `.sswapa` in your console to swap to SCP-{player.Role}, or type `.sswapd` to reject the request");
        }
        public static bool CanSwap(Player player, Player target, out string response)
        {
            if (player.Health != player.MaxHealth)
            {
                response = "You cannot swap as you have taken damage";
                return false;
            }
            else if (target != null && target.Health != target.MaxHealth)
            {
                response = "You cannot swap as the person you want to swap with has taken damage";
                return false;
            }
            else if (Round.Duration > TimeSpan.FromMinutes(1))
            {
                response = "You can only swap your SCP within the first minute of a round";
                return false;
            }

            response = string.Empty;
            return true;
        }

        public static void SwapSCPs(Player player, Player target)
        {
            RoleTypeId playerSCP = player.Role;
            RoleTypeId targetSCP = target.Role;

            player.SetRole(targetSCP, RoleChangeReason.LateJoin);
            target.SetRole(playerSCP, RoleChangeReason.LateJoin);

            RemoveSwapRequest(player);
        }

        #region Events

        public override void OnServerRoundEnded(RoundEndedEventArgs ev)
        {
            triggers.Clear();
            swapDict.Clear();
            swapRequests.Clear();
            SCPsToReplace = 0;
        }
        public override void OnServerRoundStarted()
        {
            SCPSwap.SCPsToReplace = 0;

            Timing.CallDelayed(SCPSwap.SwapToHumanSeconds, () =>
            {
                if (SCPSwap.SCPsToReplace > 0)
                {
                    var currentScps = Player.List.Where(p => p.ReferenceHub.IsSCP(false));
                    if (currentScps.Count() == 1 && (currentScps.First().Role == RoleTypeId.Scp079 || currentScps.First().Role == RoleTypeId.Scp096))
                    {
                        var newRole = SCPSwap.scpRoles.Except([RoleTypeId.Scp079, RoleTypeId.Scp096]).ToList().RandomItem();
                        currentScps.First().SetRole(newRole, RoleChangeReason.LateJoin);
                    }

                    foreach (var scpPlr in currentScps)
                    {
                        if (scpPlr.Role.IsValidSCP())
                        {
                            scpPlr.Health += 500 * SCPSwap.SCPsToReplace;
                            scpPlr.MaxHealth += 500 * SCPSwap.SCPsToReplace;
                        }
                    }

                    if (currentScps.Count() == 0)
                        Server.SendBroadcast("There are no SCPs this round.", 5, Broadcast.BroadcastFlags.Normal, true);
                    else
                        Server.SendBroadcast($"Due to {SCPSwap.SCPsToReplace} missing SCP(s), All living SCPs have been buffed", 5, Broadcast.BroadcastFlags.Normal, true);

                }
            });
        }
        public override void OnPlayerSpawned(PlayerSpawnedEventArgs ev)
        {
            if (ev.Player.Role.IsValidSCP() && Round.Duration < TimeSpan.FromMinutes(1))
            {
                if (ServerSpecificSettingsSync.TryGetSettingOfUser<SSTwoButtonsSetting>(ev.Player.ReferenceHub, (int)SettingsIDs.SCP_NeverSCP, out SSTwoButtonsSetting settings)
                    && settings.SyncIsA)
                {
                    Timing.CallDelayed(0.15f, () =>
                    {
                        SCPSwap.SwapScpToHuman(ev.Player.ReferenceHub);
                    });
                }
                else
                {
                    ev.Player.SendBroadcast("You can swap SCP with another player by running the \".scpswap <SCP>\" command in your console", 5);
                    ev.Player.SendBroadcast("You can change back to a human role by running the \".human\" command", 5);
                }
            }
        }
        public override void OnPlayerLeft(PlayerLeftEventArgs ev)
        {
            if (Round.Duration < TimeSpan.FromMinutes(1) && ev.Player.ReferenceHub.IsSCP(false))
            {
                SCPSwap.SCPsToReplace++;
                SCPSwap.ReplaceBroadcast();
            }
        }

        #endregion
    }
}
