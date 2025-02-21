using LabApi.Features.Wrappers;
using PluginAPI.Events;

namespace CustomCommands.Features.Events.SnowballFight
{
	public class SnowballEvents
	{
		[PluginEvent]
		public void PlayerDeath(PlayerDeathEvent args)
		{
			if (CustomCommandsPlugin.CurrentEvent == EventType.SnowballFight)
			{
				args.Attacker.Heal(15);
			}
		}
	}
}
