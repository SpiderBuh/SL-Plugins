using CommandSystem;
using LabApi.Features.Wrappers;
using PlayerRoles;
using RedRightHand;
using RedRightHand.Commands;
using RemoteAdmin;
using System;
using System.Linq;

namespace CustomCommands.Features.SCPSwap.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	[CommandHandler(typeof(ClientCommandHandler))]
	public class SwapParentCommand : ParentCommand
	{
		public override string Command => "swap";

		public override string[] Aliases { get; } = { "scpswap", "scp" };

		public override string Description => "Commands for the SCP Swap system";

		public override void LoadGeneratedCommands()
		{
			throw new NotImplementedException();
		}

		protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (sender is PlayerCommandSender pSender && pSender.ReferenceHub.IsSCP() && pSender.ReferenceHub.roleManager.CurrentRole.RoleTypeId != RoleTypeId.Scp0492)
			{
				var player = Player.Get(pSender.ReferenceHub);

				if(!SCPSwap.CanSwap(player, null, out response))
					return false;

				var role = Extensions.GetRoleFromString($"SCP" + arguments.Array[1]);
				if (SCPSwap.AvailableSCPs.Contains(role))
				{
					response = "You cannot swap to that SCP";
					return false;
				}

				var scpNum = player.Role.SCPNumbersFromRole();
				var target = Player.List.Where(r => r.Role == role).First();

				if (SCPSwap.HasSwapRequest(player, out var _))
				{
					response = "You already have another pending swap request";
					return false;
				}
				else if (SCPSwap.HasSwapRequest(target, out var _))
				{
					response = $"{target} is trying to swap with another player";
					return false;
				}

				SCPSwap.AddSwapRequest(player, target);
				response = "Swap Request Sent";
				return true;
			}

			response = "You must be an SCP to run this command";
			return false;
		}
	}
}
