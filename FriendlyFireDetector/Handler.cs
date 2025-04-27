using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerRoles;
using static UnityEngine.GraphicsBuffer;
using UnityEngine;
using AdminToys;
using CustomPlayerEffects;
using InventorySystem.Items;
using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items.Pickups;
using static RoundSummary;
using Mirror;
using LabApi.Events.CustomHandlers;
using FriendlyFireDetector.Data;
using LabApi.Events.Arguments.PlayerEvents;
using RedRightHand;
using Extensions = RedRightHand.Extensions;
using LabApi.Features.Wrappers;
using LabApi.Events.Arguments.ServerEvents;

namespace FriendlyFireDetector
{
	public class Handler : CustomEventsHandler
	{
		public readonly Dictionary<string, FFCount> FFCounts = new Dictionary<string, FFCount>();
		public readonly Dictionary<string, RoleTypeId> PreviousRoles = new Dictionary<string, RoleTypeId>();
		public readonly Dictionary<string, List<GrenadeThrowerInfo>> GrenadeInfo = new Dictionary<string, List<GrenadeThrowerInfo>>();
		public readonly Dictionary<string, FFInfo> FFInfo = new Dictionary<string, FFInfo>();
		public static bool Paused;

		public override void OnPlayerSpawned(PlayerSpawnedEventArgs ev)
		{
			if (!IsValidPlayer(ev.Player) || ev.Role.RoleTypeId == RoleTypeId.None)
				return;

			PreviousRoles.AddToOrReplaceValue(ev.Player.UserId, ev.Role.RoleTypeId);
		}

		public override void OnPlayerHurting(PlayerHurtingEventArgs ev)
		{
			if (FFDPlugin.Paused || !Extensions.RoundInProgress() || !IsValidPlayer(ev.Player) || !IsValidPlayer(ev.Player) || ev.Player.UserId == ev.Attacker.UserId || !(ev.DamageHandler is AttackerDamageHandler aDH))
				return;

			var atkrHasPrevRole = PreviousRoles.TryGetValue(ev.Attacker.UserId, out var atkrPrevRole);

			//Checks both the attacker's current role and possible previous roles for if it is considered FF against the target's role
			if (!ev.Player.IsFF(ev.Attacker))
				return;

			int friendlies = 0;
			int hostiles = 0;

			foreach (var plr in GetNearbyPlayers(ev.Attacker))
			{
				if (plr.IsFF(ev.Attacker))
					friendlies++;
				else
					hostiles++;
			}

			if (hostiles > 0)
				return;

			if (FFCounts.TryGetValue(ev.Attacker.UserId, out var fFCount))
			{
				if(fFCount.Count > 5 && (DateTime.Now - fFCount.LastUpdate).TotalSeconds < 10)
					ev.Attacker.Damage(2^((fFCount.Count-5)/2), "FriendlyFire reversal");
				

				fFCount.Count++;
				fFCount.LastUpdate = DateTime.UtcNow;
			}
			else
				FFCounts.AddToOrReplaceValue(ev.Player.UserId, new FFCount(1));
		}

		public override void OnServerWaitingForPlayers()
		{
			FFCounts.Clear();
			PreviousRoles.Clear();
			GrenadeInfo.Clear();
			FFInfo.Clear();
		}

		private bool IsValidPlayer(Player plr)
		{
			return plr != null && !plr.IsNpc && !plr.IsServer && plr.IsOnline && !plr.IsTutorial;
		}

		private List<Player> GetNearbyPlayers(Player atkr, bool rangeOnly = false)
		{
			float distanceCheck = atkr.Position.y > 900 ? 70 : 35;
			List<Player> nearbyPlayers = [];

			foreach (var plr in Player.List)
			{
				if (plr.IsServer || plr.Role == RoleTypeId.Spectator)
					continue;

				var distance = Vector3.Distance(atkr.Position, plr.Position);

				if (rangeOnly && distance <= distanceCheck)
					nearbyPlayers.Add(plr);
				else
				{
					var angle = Vector3.Angle(atkr.GameObject.transform.forward, atkr.Position - plr.Position);

					if ((distance <= distanceCheck && angle > 130) || distance < 5)
						nearbyPlayers.Add(plr);
				}
			}

			return nearbyPlayers;
		}
	}
}
