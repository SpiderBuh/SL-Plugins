using MEC;
using PlayerRoles;
using LabApi.Features.Wrappers;
using CustomCommands.Core;
using LabApi.Events.Arguments.PlayerEvents;


namespace CustomCommands.Features.SCP079Removal
{
	public class SCP079Removal : CustomFeature
	{
		public SCP079Removal(bool configSetting) : base(configSetting)
		{
		}

		public override void OnPlayerSpawned(PlayerSpawnedEventArgs ev)
		{
			if (ev.Role.RoleTypeId == RoleTypeId.Scp079)
			{
				Timing.CallDelayed(0.15f, () =>
				{
					ev.Player.SetRole(RoleTypeId.Scp3114, RoleChangeReason.LateJoin);
				});
			}
		}
	}
}
