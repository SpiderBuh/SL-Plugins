using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MEC;
using Newtonsoft.Json;
using PlayerRoles;
using PlayerStatsSystem;
using RedRightHand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static StatTracker.StatTrackerPlugin;

namespace StatTracker
{
	public class Events : CustomEventsHandler
	{
		public static Dictionary<string, Stats> StatData = new Dictionary<string, Stats>();
		public static Dictionary<string, bool> PlayerCuffed = new Dictionary<string, bool>();

		public override void OnServerWaitingForPlayers()
		{
			StatData.Clear();
			PlayerCuffed.Clear();
		}
		public override void OnServerRoundStarted()
		{
			foreach (var plr in Player.List)
			{
				if (StatData.ContainsKey(plr.UserId))
					StatData[plr.UserId].Jointime = DateTime.UtcNow;
				else
					StatData.Add(plr.UserId, new StatTrackerPlugin.Stats(plr));
			}
		}
		public override void OnServerRoundEnded(RoundEndedEventArgs ev)
		{
			foreach (var plr in Player.List)
			{
				if (StatData.ContainsKey(plr.UserId))
				{
					StatData[plr.UserId].SecondsPlayed += (int)(DateTime.UtcNow - StatData[plr.UserId].Jointime).TotalSeconds;

					if (plr.IsAlive)
						StatData[plr.UserId].RoundWon = (plr.Team == LeadingTeamToPlayerTeam(ev.LeadingTeam));
				}
			}

			Timing.RunCoroutine(HandleDataSend(StatData));
		}

		public override void OnPlayerSpawned(PlayerSpawnedEventArgs ev)
		{
			if (ev.Player == null || ev.Player.UserId == null)
				return;

			if (StatData.ContainsKey(ev.Player.UserId) && ev.Player.Role != RoleTypeId.Spectator)
			{
				if (StatData[ev.Player.UserId].Spawns.ContainsKey((int)ev.Player.Role))
					StatData[ev.Player.UserId].Spawns[(int)ev.Player.Role] += 1;
				else
					StatData[ev.Player.UserId].Spawns.Add((int)ev.Player.Role, 1);
			}
		}
		public override void OnPlayerJoined(PlayerJoinedEventArgs ev)
		{
			if (Extensions.RoundInProgress())
			{
				if (StatData.ContainsKey(ev.Player.UserId))
					StatData[ev.Player.UserId].Jointime = DateTime.UtcNow;
				else
					StatData.Add(ev.Player.UserId, new StatTrackerPlugin.Stats(ev.Player));
			}
		}
		public override void OnPlayerLeft(PlayerLeftEventArgs ev)
		{
			try
			{
				if (Extensions.RoundInProgress())
				{
					if (StatData.ContainsKey(ev.Player.UserId))
					{
						StatData[ev.Player.UserId].SecondsPlayed += (int)(DateTime.UtcNow - StatData[ev.Player.UserId].Jointime).TotalSeconds;
					}
				}
			}
			//To stop it throwing an error if round is null (for like forced round restarts)
			catch (Exception) { }
		}

		public override void OnPlayerHurt(PlayerHurtEventArgs ev)
		{
			if (!(ev.DamageHandler is AttackerDamageHandler aDH) || !StatData.ContainsKey(ev.Target.UserId) || !StatData.ContainsKey(aDH.Attacker.Hub.authManager.UserId))
				return;

			var targ = ev.Target;
			var atkr = Player.Get(aDH.Attacker.Hub);

			if (!aDH.IsFriendlyFire && targ.Role != RoleTypeId.ClassD)
			{
				StatData[targ.UserId].DamageTaken += (int)aDH.Damage;
				StatData[atkr.UserId].DamageDealt += (int)aDH.Damage;
			}
		}
		public override void OnPlayerDeath(PlayerDeathEventArgs ev)
		{
			if (!(ev.DamageHandler is AttackerDamageHandler aDH) || !StatData.ContainsKey(ev.Player.UserId) || !StatData.ContainsKey(aDH.Attacker.Hub.authManager.UserId))
				return;

			var targ = ev.Player;
			var atkr = Player.Get(aDH.Attacker.Hub);

			if (StatData[targ.UserId].Deaths.ContainsKey((int)targ.Role))
				StatData[targ.UserId].Deaths[(int)targ.Role] += 1;
			else
				StatData[targ.UserId].Deaths.Add((int)targ.Role, 1);

			if (StatData[atkr.UserId].Killed.ContainsKey((int)targ.Role))
				StatData[atkr.UserId].Killed[(int)targ.Role] += 1;
			else
				StatData[atkr.UserId].Killed.Add((int)targ.Role, 1);
		}
		public override void OnPlayerEscaped(PlayerEscapedEventArgs ev)
		{
			if (StatData.ContainsKey(ev.Player.UserId))
				StatData[ev.Player.UserId].Escaped = true;
		}
		public override void OnPlayerCuffed(PlayerCuffedEventArgs ev)
		{
			if (ev.Player == null || ev.Target == null || !StatData.ContainsKey(ev.Player.UserId) || !StatData.ContainsKey(ev.Target.UserId))
				return;

			if (!PlayerCuffed.ContainsKey(ev.Target.UserId))
			{
				StatData[ev.Player.UserId].PlayersDisarmed += 1;
				PlayerCuffed.Add(ev.Target.UserId, true);
			}
		}
		public override void OnPlayerUsedItem(PlayerUsedItemEventArgs ev)
		{
			if (ev.Item.Category != ItemCategory.Medical || !StatData.ContainsKey(ev.Player.UserId))
				return;

			StatData[ev.Player.UserId].MedicalItems += 1;
		}

		//Gets the data and sends it off to the API for handling.
		//This is done as a co-routine so that it doesn't interfere with normal server running.

		public IEnumerator<float> HandleDataSend(Dictionary<string, Stats> StatData)
		{
			List<Stats> stats = new List<Stats>();

			foreach (var a in StatData)
			{
				Logger.Info($"Adding {a.Key} | {a.Value.UserID} | {a.Value.DNT}");

				if (!a.Value.DNT)
				{
					stats.Add(a.Value);
				}
			}

			Logger.Info($"Sending stat data for {stats.Count} players");

			var json = JsonConvert.SerializeObject(stats.ToArray(), Formatting.Indented);
			_ = Post(StatTrackerPlugin.config.ApiEndpoint, new StringContent(json, Encoding.UTF8, "application/json"));

			yield return 0f;
		}

		public async static Task<HttpResponseMessage> Post(string Url, StringContent Content)
		{
			using (HttpClient client = new HttpClient())
			{
				client.BaseAddress = new Uri(Url);

				return await client.PostAsync(client.BaseAddress, Content);
			}
		}

		#region Misc Data Methods

		public Team LeadingTeamToPlayerTeam(RoundSummary.LeadingTeam lTeam)
		{
			switch (lTeam)
			{
				case RoundSummary.LeadingTeam.FacilityForces: return Team.FoundationForces;
				case RoundSummary.LeadingTeam.ChaosInsurgency: return Team.ChaosInsurgency;
				case RoundSummary.LeadingTeam.Anomalies: return Team.ChaosInsurgency;
				default: return Team.Dead;
			}

			#endregion

		}
	}
}
