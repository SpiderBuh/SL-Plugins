using CustomCommands.Core;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using RedRightHand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CustomCommands.Features.CustomRoles.CustomRolesManager;

namespace CustomCommands.Features.CustomRoles.Roles
{
	public abstract class CustomRoleBase
	{
		public abstract CustomRoleType CustomRole { get; }
		public abstract string Name { get; }

		public virtual void EnableRole(Player player)
		{
			ActiveRoles.AddToOrReplaceValue(player.UserId, CustomRole);
		}

		public virtual void DisableRole(Player player)
		{
			ActiveRoles.Remove(player.UserId);
		}

		public virtual bool ToggleRole(Player player)
		{
			if (ActiveRoles.ContainsKey(player.UserId))
			{
				DisableRole(player);
				return false;
			}
			else
			{
				EnableRole(player);
				return true;
			}
		}

		public virtual void PlayerSpawned(PlayerSpawnedEventArgs ev) { }
		public virtual void PlayerHurt(PlayerHurtingEventArgs ev) { }
	}
}
