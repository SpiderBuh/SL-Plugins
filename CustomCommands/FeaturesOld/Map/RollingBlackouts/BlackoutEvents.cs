using Interactables.Interobjects.DoorUtils;
using LabApi.Features.Wrappers;
using PluginAPI.Events;

namespace CustomCommands.Features.Map.RollingBlackouts
{
	public class BlackoutEvents
	{
		[PluginEvent]
		public bool PlayerInteractDoor(PlayerInteractDoorEvent ev)
		{
			if (!CustomCommandsPlugin.EventInProgress && RoomLightController.IsInDarkenedRoom(ev.Player.Position) && ev.Door.RequiredPermissions.RequiredPermissions == KeycardPermissions.None)
				return false;

			return ev.CanOpen;
		}

		[PluginEvent]
		public void RoundStartEvent(RoundStartEvent ev)
		{
			BlackoutManager.DelayThisRound = UnityEngine.Random.Range(CustomCommandsPlugin.Config.MinBlackoutTime, CustomCommandsPlugin.Config.MaxBlackoutTime);
			BlackoutManager.TriggeredThisRound = false;
		}
	}
}
