using CommandSystem;
using LabApi.Features.Wrappers;
using PlayerRoles;
using RedRightHand.Commands;
using RemoteAdmin;
using System;

namespace CustomCommands.Features.SCPSwap.Commands
{
	[CommandHandler(typeof(SwapParentCommand))]
	public class Accept : ICustomCommand
	{
		public string Command => "accept";

		public string[] Aliases { get; } = { "a" };
		public string Description => "Accepts your pending swap request";

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

				if (!SCPSwap.CanSwap(player, swapper, out response))
					return false;

				swapper.SendHint($"{player.Nickname} accepted your swap request", 5);

				SCPSwap.SwapSCPs(player, swapper);

				response = "Request accepted";
				return true;
			}

			response = "You must be an SCP to run this command";
			return false;
		}
	}
}
