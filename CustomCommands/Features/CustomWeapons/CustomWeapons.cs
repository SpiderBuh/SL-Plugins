using CustomCommands.Core;
using CustomCommands.Features.CustomWeapons.Weapons;
using LabApi.Events.Arguments.PlayerEvents;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Features.CustomWeapons
{
	public class CustomWeapons : CustomFeature
	{
		public static RoleTypeId[] RagdollRoles = new RoleTypeId[]
		{
					RoleTypeId.ClassD, RoleTypeId.Scientist, RoleTypeId.Scp049, RoleTypeId.Scp0492, RoleTypeId.ChaosConscript, RoleTypeId.ChaosMarauder, RoleTypeId.ChaosRepressor, RoleTypeId.ChaosRepressor,
					RoleTypeId.NtfCaptain, RoleTypeId.NtfSpecialist, RoleTypeId.NtfPrivate, RoleTypeId.NtfSergeant, RoleTypeId.Tutorial
		};

		public static Dictionary<string, WeaponType> EnabledWeapons = new Dictionary<string, WeaponType>();
		public static Dictionary<WeaponType, CustomWeaponBase> AvailableWeapons = new Dictionary<WeaponType, CustomWeaponBase>();

		public enum WeaponType
		{
			NONE, Grenade, Flashbang, Ball, Ragdoll
		}

		public CustomWeapons(bool configSetting) : base(configSetting)
		{
			if (isEnabled)
			{
				AvailableWeapons = new Dictionary<WeaponType, CustomWeaponBase>
				{
					{ WeaponType.Grenade,  new GrenadeLauncher() },
					{ WeaponType.Flashbang, new FlashLauncher() },
					{ WeaponType.Ball, new BallLauncher() },
					{ WeaponType.Ragdoll, new RagdollLauncher() },
				};
			}
		}

		public override void OnPlayerShootingWeapon(PlayerShootingWeaponEventArgs ev)
		{
			if (EnabledWeapons.TryGetValue(ev.Player.UserId, out WeaponType type) && AvailableWeapons.TryGetValue(type, out var wepBase))
			{
				wepBase.ShootWeapon(ev.Player);
			}
		}

		public override void OnPlayerHurting(PlayerHurtingEventArgs ev)
		{
			if (EnabledWeapons.TryGetValue(ev.Player.UserId, out WeaponType type) && AvailableWeapons.TryGetValue(type, out var wepBase))
			{
				wepBase.HitPlayer(ev);
			}
		}

		public override void OnServerWaitingForPlayers()
		{
			EnabledWeapons.Clear();
		}

	}
}
