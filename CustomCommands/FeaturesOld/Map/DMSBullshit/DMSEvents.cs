
using LabApi.Features.Wrappers;
using PluginAPI.Events;
using RedRightHand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Features.Map.DMSBullshit
{
	public class DMSEvents
	{
		[PluginEvent]
		public void OnRoundStart(RoundStartEvent ev)
		{
			typeof(DeadmanSwitch).UpdatePrivateProperty("_dmsDelay", UnityEngine.Random.Range(90, 450));
		}
	}
}
