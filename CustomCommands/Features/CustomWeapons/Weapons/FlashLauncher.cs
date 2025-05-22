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
	public class FlashLauncher : CustomWeaponBase
	{
		public override CustomWeapons.WeaponType WeaponType => CustomWeapons.WeaponType.Flashbang;

		public override string Name => "Flashbang Launcher";

		public override void ShootWeapon(Player player)
		{
			Helpers.SpawnGrenade<FlashbangGrenade>(player, ItemType.GrenadeFlash, Helpers.RandomThrowableVelocity(player.Camera.transform));
		}
	}
}
