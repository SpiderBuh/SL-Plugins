using CommandSystem;
using LabApi.Features.Wrappers;
using PlayerRoles;

using RedRightHand.Commands;
using RemoteAdmin;
using System;

namespace CustomCommands.Features.SCPSwap.Commands
{
	[CommandHandler(typeof(SwapParentCommand))]
	public class Deny : ICustomCommand
	{
		public string Command => "deny";

		public string[] Aliases { get; } = { "d" };
		public string Description => "Denies your pending swap request";

		public string[] Usage => null;

		public PlayerPermissions? Permission => null;

		public string PermissionString => string.Empty;

		public bool RequirePlayerSender => true;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (sender is PlayerCommandSender pSender && pSender.ReferenceHub.IsSCP() && pSender.ReferenceHub.roleManager.CurrentRole.RoleTypeId != RoleTypeId.Scp0492)
			{
				var player = Player.Get(pSender.ReferenceHub);

				if (!SCPSwap.HasSwapRequest(player, out var swapper))
				{
					response = "You do not have a pending swap request";
					return false;
				}

				swapper.SendHint($"{player.Nickname} denied your swap request", 5);
				SCPSwap.RemoveSwapRequest(swapper);

				response = "Request denied";
				return true;
			}

			response = "You must be an SCP to run this command";
			return false;
		}
	}
}
