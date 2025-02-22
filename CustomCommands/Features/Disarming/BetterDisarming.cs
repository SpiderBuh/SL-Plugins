using PlayerRoles;
using PlayerStatsSystem;
using LabApi.Features.Wrappers;

using RedRightHand;
using System.Linq;
using CustomCommands.Core;
using LabApi.Events.Arguments.PlayerEvents;
using System.Collections.Generic;

namespace CustomCommands.Features.Disarming
{
	public class BetterDisarming : CustomFeature
	{
		List<string> disarmedUsers = new List<string>();
		Dictionary<string, int> kosDict = new Dictionary<string, int>();

		public BetterDisarming(bool configSetting) : base(configSetting)
		{
		}

		public override void OnPlayerCuffed(PlayerCuffedEventArgs ev)
		{
			if(ev.Target.Role == RoleTypeId.ClassD && !disarmedUsers.Contains(ev.Target.UserId))
				disarmedUsers.Add(ev.Target.UserId);
		}

		public override void OnPlayerHurting(PlayerHurtingEventArgs ev)
		{
			if (ev.DamageHandler is FirearmDamageHandler fDH)
			{
				var isVicClassD = ev.Target.Role == RoleTypeId.ClassD;
				var isAtkrFacGuard = ev.Player.Role == RoleTypeId.FacilityGuard;
				var hasVicDisarmed = !disarmedUsers.Contains(ev.Target.UserId);
				var hasExclusionItems = !ev.Target.ReferenceHub.inventory.UserInventory.Items.Where(i =>
					i.Value.Category == ItemCategory.Firearm ||
					i.Value.Category == ItemCategory.SpecialWeapon ||
					(i.Value.Category == ItemCategory.SCPItem && i.Value.ItemTypeId != ItemType.SCP330) ||
					i.Value.Category == ItemCategory.Grenade).Any();

				if (isVicClassD && isAtkrFacGuard && hasVicDisarmed && hasExclusionItems)
				{
					fDH.UpdatePrivateProperty("Damage", fDH.Damage / 2);
				}
			}
		}

		public override void OnServerWaitingForPlayers()
		{
			kosDict.Clear();
			disarmedUsers.Clear();
		}
	}
}
