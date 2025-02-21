using PlayerRoles;
using LabApi.Features.Wrappers;
using PluginAPI.Events;
using Utils;

namespace CustomCommands.Features.Humans.TutorialFix
{
	public class TutorialEvents
	{
		[PluginEvent]
		public bool OnDisarm(PlayerHandcuffEvent args)
		{
			if (args.Target.Role == RoleTypeId.Tutorial)
				return false;
			else
				return true;
		}

		[PluginEvent]
		public void CoinFlip(PlayerCoinFlipEvent args)
		{
			if (args.Player.Role == RoleTypeId.Tutorial && CustomCommandsPlugin.Config.TutorialCoinExplosion)
			{
				MEC.Timing.CallDelayed(2, () =>
				{
					if (!args.IsTails)
					{
						ExplosionUtils.ServerExplode(args.Player.ReferenceHub, ExplosionType.PinkCandy);
					}
				});
			}
		}
	}
}
