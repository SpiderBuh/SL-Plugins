using InventorySystem.Items.ThrowableProjectiles;
using LabApi.Features.Wrappers;
using RedRightHand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Features.CustomWeapons.Weapons
{
	public class GrenadeLauncher : CustomWeaponBase
	{
		public override CustomWeapons.WeaponType WeaponType => CustomWeapons.WeaponType.Grenade;

		public override string Name => "Grenade Launcher";

		public override void ShootWeapon(Player player)
		{
			Helpers.SpawnGrenade<TimeGrenade>(player, ItemType.GrenadeHE, Helpers.RandomThrowableVelocity(player.Camera.transform));
		}
	}
}
