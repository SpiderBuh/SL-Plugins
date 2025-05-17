using CustomCommands.Core;
using CustomCommands.Features.SurfaceLightFix;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabApi.Features.Wrappers;
using UnityEngine;
using LabApi.Events.Arguments.PlayerEvents;
using Logger = LabApi.Features.Console.Logger;

namespace CustomCommands.Features.SurfaceLightingFix
{
	public class SurfaceLightingFix : CustomFeature
	{
		public SurfaceLightingFix(bool configSetting) : base(configSetting)
		{
		}

		public override void OnServerWaitingForPlayers()
		{
			//foreach(var p in NetworkClient.prefabs)
			//{
			//	Logger.Info($"{p.Value.name}");
			//}

			//Map.SetColorOfLights(new Color(0.7f, 0.7f, 0.7f));

			//var rand = new System.Random();

			//foreach(var room in Map.Rooms.Where(r => r.Zone == MapGeneration.FacilityZone.HeavyContainment))
			//{
			//	if(rand.Next(0, 101) < 9)
			//	{
			//		room.LightController.FlickerLights(100000);
			//	}
			//}

			GameObject obj = new GameObject();
			obj.AddComponent<SurfaceLightObject>();
			NetworkServer.Spawn(obj);
		}
	}
}
