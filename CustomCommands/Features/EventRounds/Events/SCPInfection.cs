using Interactables.Interobjects.DoorUtils;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using MapGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomCommands.Features.EventRounds.Events
{
	public class SCPInfection : CustomEventRound
	{
		public override void OnServerRoundStarted()
		{
			Round.IsLocked = true;

			var room = RoomIdentifier.AllRoomIdentifiers.Where(r => r.Name == RoomName.Lcz914);
			var room914 = room.First();

			MEC.Timing.CallDelayed(10, () =>
			{
				foreach (var door in DoorVariant.DoorsByRoom[room914])
				{
					door.NetworkTargetState = true;
				}
			});

			int scps = Mathf.Clamp((int)Math.Floor((double)(Player.Count / 5)), 1, 5);
			int remainingPlayers = Player.Count - 1;
			var rand = new System.Random();	

			foreach (Player plr in Player.GetAll())
			{
				if (plr.IsHost || plr.Role == PlayerRoles.RoleTypeId.Overwatch)
					continue;

				if ((scps > 0 && rand.Next(0, 2) == 1) || remainingPlayers < scps)
				{
					switch (rand.Next(0, 5))
					{
						default:
							plr.SetRole(PlayerRoles.RoleTypeId.Scp173);
							break;
						case 1:
							plr.SetRole(PlayerRoles.RoleTypeId.Scp049);
							break;
						case 2:
							plr.SetRole(PlayerRoles.RoleTypeId.Scp939);
							break;
						case 4:
							plr.SetRole(PlayerRoles.RoleTypeId.Scp096);
							break;
					}
					plr.Position = new Vector3(room914.transform.position.x, room914.transform.position.y + 1, room914.transform.position.z);
					plr.ClearBroadcasts();
					plr.SendBroadcast("The door will open in 10 seconds. Kill them all", 10, shouldClearPrevious: true);
					scps--;
				}
				else
				{
					plr.SetRole(PlayerRoles.RoleTypeId.ClassD);
					plr.SendBroadcast("The hunters will release in 10 seconds", 10, shouldClearPrevious: true);
				}
				remainingPlayers--;
			}

			Round.IsLocked = false;
		}
		public override void OnPlayerDying(PlayerDyingEventArgs ev)
		{
			ev.Player.SetRole(ev.Attacker.Role, PlayerRoles.RoleChangeReason.Revived, PlayerRoles.RoleSpawnFlags.AssignInventory);
		}
	}
}
