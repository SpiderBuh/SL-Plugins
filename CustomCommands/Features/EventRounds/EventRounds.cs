using CustomCommands.Core;
using CustomCommands.Features.EventRounds.Events;
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
			CurrentEvent = EventHandlers[QueuedEvent];

			if (QueuedEvent != EventType.NONE)
				QueuedEvent = EventType.NONE;
		}
	}
}
