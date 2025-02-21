using CustomCommands.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Features.EventRounds
{
	public class EventRounds : CustomFeature
	{
		private static EventType eventType;
		public static bool EventInProgress => eventType == EventType.NONE;
		private CustomEventRound eventRound;

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

		public void setEventRound(EventType type)
		{
			switch (type)
			{
				default:
					eventRound = null;
					break;
			}
		}
	}
}
