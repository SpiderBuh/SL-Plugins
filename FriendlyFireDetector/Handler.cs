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
using RedRightHand.DataStores;
using LabApi.Features.Console;
using Logger = LabApi.Features.Console.Logger;

namespace FriendlyFireDetector
{
	public class Handler : CustomEventsHandler
	{
		public static bool Paused;

		public override void OnPlayerChangedRole(PlayerChangedRoleEventArgs ev)
		{
			if (!IsValidPlayer(ev.Player) || ev.NewRole.RoleTypeId == RoleTypeId.None)
				return;

			var ffdData = ev.Player.GetDataStore<FFDStore>();
			ffdData.PreviousRole = ev.OldRole;
		}

		public override void OnPlayerHurting(PlayerHurtingEventArgs ev)
		{
			if (FFDPlugin.Paused || !Extensions.RoundInProgress() || !IsValidPlayer(ev.Player) || !IsValidPlayer(ev.Attacker) || ev.Player.UserId == ev.Attacker.UserId 
				|| !(ev.DamageHandler is AttackerDamageHandler aDH))
				return;

			var atkrStore = ev.Attacker.GetDataStore<FFDStore>();

			//Checks both the attacker's current role and possible previous roles for if it is considered FF against the target's role
			if (!ev.Attacker.IsFF(ev.Player, atkrStore.PreviousRole != RoleTypeId.None))
				return;

			int friendlies = 0;
			int hostiles = 0;

			foreach (var plr in GetNearbyPlayers(ev.Attacker))
			{
				if (ev.Attacker.IsFF(plr, false))
					friendlies++;
				else
					hostiles++;
			}

			if (hostiles > 0)
				return;

			ev.IsAllowed = false;
			atkrStore.BlockLog = true;

			if (atkrStore.ReverseFFEnabled)
			{
				ev.Attacker.Damage(aDH.Damage, ev.Attacker, armorPenetration: 100);
			}
			else
			{
				//Logger.Info($"{2 ^ ((atkrStore.TriggerCount - 5))}");

				if (atkrStore.TriggerCount > 5 && (DateTime.Now - atkrStore.LastTrigger).TotalSeconds < 10)
					ev.Attacker.Damage(2 ^ ((atkrStore.TriggerCount - 5)), "FriendlyFire reversal");
			}


			atkrStore.TriggerCount++;
			atkrStore.LastTrigger = DateTime.UtcNow;
		}

		private bool IsValidPlayer(Player plr)
		{
			return plr != null /*&& !plr.IsNpc */&& !plr.IsServer && plr.IsOnline && !plr.IsTutorial;
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
