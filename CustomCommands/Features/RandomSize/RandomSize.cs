using CustomCommands.Core;
using CustomCommands.Features.Players.Size;
using LabApi.Events.Arguments.PlayerEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Features.RandomSize
{
	public class RandomSize : CustomFeature
	{
		public RandomSize(bool configSetting) : base(configSetting)
		{
		}

		public override void OnPlayerSpawned(PlayerSpawnedEventArgs ev)
		{
			var size = UnityEngine.Random.Range(0.9f, 1.1f);
			ev.Player.SetSize(size, size, size);
		}
	}
}
