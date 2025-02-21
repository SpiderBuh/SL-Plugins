using PlayerRoles;
using PlayerStatsSystem;
using LabApi.Features.Wrappers;
using LabApi.Events;
using PluginAPI.Events;

namespace CustomCommands.Features.Events.Infection
{
	public class InfectionEvents
	{
		[PluginEvent, PluginPriority(LoadPriority.Highest)]
		public bool PlayerDying(PlayerDyingEvent args)
		{
			if (CustomCommandsPlugin.CurrentEvent == EventType.Infection && args.DamageHandler is AttackerDamageHandler)
			{
				args.Player.ReferenceHub.roleManager.ServerSetRole(args.Attacker.Role, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.AssignInventory);

				return false;
			}

			return true;
		}
	}
}
