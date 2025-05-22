using CustomCommands.Core;
using CustomCommands.Features.EventRounds.Events;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.Scp914Events;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Features.EventRounds
{
	public class EventRounds : CustomFeature
	{
		private Dictionary<EventType, CustomEventRound> EventHandlers = new Dictionary<EventType, CustomEventRound>()
		{
		};

		private static EventType QueuedEvent;
		public static bool EventInProgress => CurrentEvent == null;
		private static CustomEventRound CurrentEvent;

		public EventRounds(bool configSetting) : base(configSetting)
		{
		}

		public enum EventType
		{
			NONE = 0,

			Infection = 1,
			Battle = 2,
			Hush = 3,
			SnowballFight = 4 // This event is christmas-exclusive.
		}

		public void QueueEvent(EventType type)
		{
			QueuedEvent = type;
		}

		public override void OnServerWaitingForPlayers()
		{
			if (QueuedEvent != EventType.NONE)
			{
				Round.IsLobbyLocked = true;
				CurrentEvent = EventHandlers[QueuedEvent];

				MEC.Timing.CallDelayed(15f, () =>
				{
					Round.IsLobbyLocked = false;
				});

			}
			else if (QueuedEvent == EventType.NONE)
				CurrentEvent = null;

			if (QueuedEvent != EventType.NONE)
				QueuedEvent = EventType.NONE;
		}

		public override void OnServerWaveTeamSelecting(WaveTeamSelectingEventArgs ev) => CurrentEvent?.OnServerWaveTeamSelecting(ev);
		public override void OnPlayerInteractingDoor(PlayerInteractingDoorEventArgs ev) => CurrentEvent?.OnPlayerInteractingDoor(ev);
		public override void OnScp914KnobChanging(Scp914KnobChangingEventArgs ev) => CurrentEvent?.OnScp914KnobChanging(ev);
		public override void OnScp914Activating(Scp914ActivatingEventArgs ev) => CurrentEvent?.OnScp914Activating(ev);
		public override void OnPlayerInteractingElevator(PlayerInteractingElevatorEventArgs ev) => CurrentEvent?.OnPlayerInteractingElevator(ev);
		public override void OnServerRoundRestarted() => CurrentEvent?.OnServerRoundRestarted();
	}
}
