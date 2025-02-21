using PlayerRoles.RoleAssign;

using LabApi.Features.Wrappers;
using LabApi.Events;
using PluginAPI.Events;


namespace CustomCommands.Features.Humans.LateJoin
{
	public class LateJoinEvents
	{
		[PluginEvent, PluginPriority(LoadPriority.Low)]
		public void PlayerJoin(PlayerJoinedEvent args)
		{
			if (!CustomCommandsPlugin.EventInProgress && Round.IsRoundStarted && Round.Duration.TotalSeconds < CustomCommandsPlugin.Config.LateJoinTime)
				HumanSpawner.SpawnLate(args.Player.ReferenceHub);
		}
	}
}
