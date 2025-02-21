using Interactables.Interobjects.DoorUtils;
using LabApi.Features.Wrappers;
using PluginAPI.Events;

namespace CustomCommands.Features.Events
{
	public class GlobalEvents
	{

		[PluginEvent]
		public void RoundRestart(RoundRestartEvent ev)
		{
			CustomCommandsPlugin.CurrentEvent = EventType.NONE;
		}

		[PluginEvent]
		public bool TeamRespawn(TeamRespawnEvent args)
		{
			if (CustomCommandsPlugin.EventInProgress)
				return false;
			else return true;
		}

		[PluginEvent]
		public bool PlayerInteractDoor(PlayerInteractDoorEvent args)
		{
			if (CustomCommandsPlugin.EventInProgress)
			{
				if (args.Door.RequiredPermissions.RequiredPermissions == KeycardPermissions.None)
					return true;
				else return false;
			}
			return args.CanOpen;
		}

		[PluginEvent]
		public bool SCP914Activate(Scp914ActivateEvent args)
		{
			if (CustomCommandsPlugin.EventInProgress)
				return false;
			else return true;
		}

		[PluginEvent]
		public bool PlayerInteractElevator(PlayerInteractElevatorEvent args)
		{
			if (CustomCommandsPlugin.EventInProgress)
			{
				return false;
			}
			else
			{
				if (args.Player.TemporaryData.Contains("plock"))
				{
					return false;
				}

				return true;
			}
		}
	}
}
