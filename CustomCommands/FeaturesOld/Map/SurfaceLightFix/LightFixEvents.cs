using Mirror;
using LabApi.Features.Wrappers;
using PluginAPI.Events;
using UnityEngine;

namespace CustomCommands.Features.Map.SurfaceLightFix
{
	public class LightFixEvents
	{
		[PluginEvent()]
		public void SpawnLights(WaitingForPlayersEvent ev)
		{
			GameObject obj = new GameObject();
			obj.AddComponent<SurfaceLightObject>();
			NetworkServer.Spawn(obj);
		}
	}
}
