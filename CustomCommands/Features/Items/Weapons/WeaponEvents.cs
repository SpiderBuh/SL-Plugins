﻿using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.Ragdolls;
using PlayerStatsSystem;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using CustomCommands.Features.Ragdoll;
using PluginAPI.Core.Items;
using InventorySystem;
using PluginAPI.Core;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Pickups;

namespace CustomCommands.Features.Items.Weapons
{
	public class WeaponEvents
	{
		RoleTypeId[] RagdollRoles = new RoleTypeId[]
		{
			RoleTypeId.ClassD, RoleTypeId.Scientist, RoleTypeId.Scp049, RoleTypeId.Scp0492, RoleTypeId.ChaosConscript, RoleTypeId.ChaosMarauder, RoleTypeId.ChaosRepressor, RoleTypeId.ChaosRepressor,
			RoleTypeId.NtfCaptain, RoleTypeId.NtfSpecialist, RoleTypeId.NtfPrivate, RoleTypeId.NtfSergeant, RoleTypeId.Tutorial
		};

		[PluginEvent]
		public void PlayerShootEvent(PlayerShotWeaponEvent args)
		{
			var plr = args.Player;

			if (plr.Role == RoleTypeId.Tutorial)
			{
				if (plr.TemporaryData.Contains("flauncher") && Plugin.Config.EnableFlashbangLauncher)
				{
					ItemManager.SpawnGrenade<FlashbangGrenade>(plr, ItemType.GrenadeFlash, ItemManager.RandomThrowableVelocity(args.Player.Camera.transform));
				}
				else if (plr.TemporaryData.Contains("glauncher") && Plugin.Config.EnableGrenadeLauncher)
				{
					ItemManager.SpawnGrenade<TimeGrenade>(plr, ItemType.GrenadeHE, ItemManager.RandomThrowableVelocity(args.Player.Camera.transform));
				}
				else if (plr.TemporaryData.Contains("blauncher") && Plugin.Config.EnableBallLauncher)
				{
					ItemManager.SpawnGrenade<Scp018Projectile>(plr, ItemType.SCP018, ItemManager.RandomThrowableVelocity(args.Player.Camera.transform));
				}

				else if (plr.TemporaryData.Contains("rdlauncher") && Plugin.Config.EnableRagdollLauncher)
				{
					var role = RagdollRoles[Random.Range(0, RagdollRoles.Length - 1)];

					PlayerRoleLoader.TryGetRoleTemplate(role, out FpcStandardRoleBase pRB);

					var dh = new FirearmDamageHandler(args.Firearm, 10);

					Vector3 velocity = Vector3.zero;
					velocity += args.Player.Camera.transform.forward * Random.Range(5f, 10f);
					velocity += args.Player.Camera.transform.up * Random.Range(0.75f, 4.5f);

					if (Random.Range(1, 3) % 2 == 0)
						velocity += args.Player.Camera.transform.right * Random.Range(0.1f, 2.5f);

					else
						velocity += args.Player.Camera.transform.right * -Random.Range(0.1f, 2.5f);

					typeof(StandardDamageHandler).GetField("StartVelocity", BindingFlags.NonPublic | BindingFlags.Instance)
						.SetValue(dh, velocity);

					RagdollData data = new RagdollData(null, dh, role, plr.Position, plr.GameObject.transform.rotation, plr.Nickname, NetworkTime.time);
					BasicRagdoll basicRagdoll = Object.Instantiate(pRB.Ragdoll);
					basicRagdoll.NetworkInfo = data;
					NetworkServer.Spawn(basicRagdoll.gameObject, (NetworkConnection)null);
				}
			}
		}

		[PluginEvent]
		public bool PlayerAttackEvent(PlayerDamageEvent ev)
		{
			if (ev.Player == null || ev.Target == null || !(ev.DamageHandler is AttackerDamageHandler aDH))
				return true;

			if (Plugin.Config.EnableTranqGun)
			{
				if (ev.Player.CurrentItem.ItemSerial == ItemManager.TranqGunSerial && ev.Target.IsHuman)
				{
					ev.Player.ReceiveHitMarker();
					ev.Target.RagdollPlayerTranqGun(ev.Player, 4);
					return false;
				}
			}

			return true;
		}

		[PluginEvent]
		public bool PlayerReloadEvent(PlayerReloadWeaponEvent ev)
		{
			if (!Round.IsRoundStarted)
				return true;

			if (Plugin.Config.EnableTranqGun && ItemManager.TranqGunSerial == 0)
			{
				if (ev.Firearm.ItemSerial == ItemManager.TranqGunSerial)
				{
					ev.Player.ReceiveHint("<voffset=1em>You cannot reload this weapon</voffset>");
					return false;
				}
			}
			return true;
		}

		[PluginEvent]
		public void PlayerChangeItemEvent(PlayerChangeItemEvent ev)
		{
			if (!Round.IsRoundStarted)
				return;

			if (Plugin.Config.EnableTranqGun && ItemManager.TranqGunSerial == 0)
			{
				if (ev.NewItem == ItemManager.TranqGunSerial)
				{
					ev.Player.ReceiveHint("<voffset=1em>You equipped the tranquilizer. It cannot be reloaded</voffset>", 8);
				}
			}
		}

		[PluginEvent]
		public void ItemPickup(PlayerSearchedPickupEvent ev)
		{
			if(Plugin.Config.EnableTranqGun && ev.Item.Info.ItemId == ItemType.GunCOM18)
			{
				if(ItemManager.TranqGunSerial == 0 || ev.Item.Info.Serial == ItemManager.TranqGunSerial)
				{
					ItemManager.TranqGunSerial = ev.Item.Info.Serial;

					ev.Player.ReceiveHint("<voffset=1em>You picked up the tranquilizer</voffset>", 8);
				}
			}
		}

		[PluginEvent]
		public void RoundStart(RoundStartEvent ev)
		{
			ItemManager.TranqGunSerial = 0;
		}
	}
}
