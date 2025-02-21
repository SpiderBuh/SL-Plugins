using CustomCommands.Core;
using CustomCommands.Features.SurfaceLightFix;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomCommands.Features.SurfaceLightingFix
{
	public class SurfaceLightingFix : CustomFeature
	{
		public SurfaceLightingFix(bool configSetting) : base(configSetting)
		{
		}

		public override void OnServerWaitingForPlayers()
		{
			GameObject obj = new GameObject();
			obj.AddComponent<SurfaceLightObject>();
			NetworkServer.Spawn(obj);
		}
	}
}
