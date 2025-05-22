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
	public class TDMInfection : CustomEventRound
	{
		RoomName[] Rooms =
		{
			RoomName.Lcz330, RoomName.Lcz173, RoomName.Lcz914, RoomName.LczClassDSpawn, RoomName.LczGlassroom, RoomName.LczGreenhouse
		};

		public override void OnServerRoundStarted()
		{
			Round.IsLocked = true;

			var rand = new System.Random();

			var room = RoomIdentifier.AllRoomIdentifiers.Where(r => r.Name == Rooms[rand.Next(Rooms.Length)]);
			var NtfSpawn = room.First();

			RollChaos:
			room = RoomIdentifier.AllRoomIdentifiers.Where(r => r.Name == Rooms[rand.Next(Rooms.Length)]);
			var ChaosSpawn = room.First();

			if(ChaosSpawn.Name == NtfSpawn.Name)
				//I don't care if it's tacky or bad practice. It's just easier here.
				goto RollChaos;
			
			MEC.Timing.CallDelayed(10, () =>
			{
				foreach (var door in DoorVariant.DoorsByRoom[NtfSpawn])
				{
					door.NetworkTargetState = true;
				}
			});
			//Allows for chaos to be freed a bit earlier than NTF, and to spread out across LCZ a bit before the NTF are freed
			MEC.Timing.CallDelayed(1, () =>
			{
				foreach (var door in DoorVariant.DoorsByRoom[ChaosSpawn])
				{
					door.NetworkTargetState = true;
				}
			});

			int index = 0;

			foreach (Player plr in Player.GetAll())
			{
				if (plr.IsHost || plr.Role == PlayerRoles.RoleTypeId.Overwatch)
					continue;

				index++;

				if(index %2 == 0)
				{
					plr.SetRole(PlayerRoles.RoleTypeId.NtfCaptain);
					plr.Position = new Vector3(NtfSpawn.transform.position.x, NtfSpawn.transform.position.x + 1, NtfSpawn.transform.position.x);
					plr.SendBroadcast("The door will open in 10 seconds. Kill all chaos", 10, shouldClearPrevious: true);
				}
				else
				{
					plr.SetRole(PlayerRoles.RoleTypeId.ChaosMarauder);
					plr.Position = new Vector3(ChaosSpawn.transform.position.x, ChaosSpawn.transform.position.x + 1, ChaosSpawn.transform.position.x);
				}
			}

			Round.IsLocked = false;
		}
		public override void OnPlayerDying(PlayerDyingEventArgs ev)
		{
			ev.Player.SetRole(ev.Attacker.Role, PlayerRoles.RoleChangeReason.Revived, PlayerRoles.RoleSpawnFlags.AssignInventory);
		}
	}
}
