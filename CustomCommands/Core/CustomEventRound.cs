using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.Scp914Events;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.CustomHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Core
{
	public class CustomEventRound : CustomEventsHandler
	{
		public override void OnServerWaveTeamSelecting(WaveTeamSelectingEventArgs ev)
		{
			ev.IsAllowed = false;
		}

		public override void OnPlayerInteractingDoor(PlayerInteractingDoorEventArgs ev)
		{
			if (ev.Door.Permissions != Interactables.Interobjects.DoorUtils.KeycardPermissions.None)
				ev.IsAllowed = false;
		}

		public override void OnScp914KnobChanging(Scp914KnobChangingEventArgs ev)
		{
			ev.IsAllowed = false;
		}

		public override void OnScp914Activating(Scp914ActivatingEventArgs ev)
		{
			ev.IsAllowed = false;
		}

		public override void OnPlayerInteractingElevator(PlayerInteractingElevatorEventArgs ev)
		{
			ev.IsAllowed = false;
		}
	}
}
