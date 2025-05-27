using CustomCommands.Core;
using CustomCommands.Features.CustomRoles.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Features.CustomRoles
{
	public class CustomRolesManager : CustomFeature
	{
		//Ideas
		//Medic: Has extra medical items, and can heal other players more efficiently.
		//Tank: Has a size of 1.2, has a max HP of 120, and heavy armour, but moves slightly slower.
		//Scout: A smaller size of 0.85, has a max HP of 85, no armour, a mp7, but move at 2x cola speed.
		//Hacker: Has a custom card that can disable tesla gates for 8 seconds, and can open any door locked by 079/blackouts

		public enum CustomRoleType
		{
			NONE, Medic, Tank, Scout, Hacker
		}

		public static Dictionary<string, CustomRoleType> ActiveRoles = new Dictionary<string, CustomRoleType>();
		public static Dictionary<CustomRoleType, CustomRoleBase> AvailableRoles = new Dictionary<CustomRoleType, CustomRoleBase>();

		public CustomRolesManager(bool configSetting) : base(configSetting)
		{
			if (isEnabled)
			{
				AvailableRoles = new Dictionary<CustomRoleType, CustomRoleBase>
				{
				};
			}
		}

		public override void OnServerWaitingForPlayers()
		{
			ActiveRoles.Clear();
		}
	}
}
