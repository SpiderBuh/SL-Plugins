using CommandSystem;
using LabApi.Features.Wrappers;
using PlayerRoles;
using RedRightHand.Commands;
using RemoteAdmin;
using System;

namespace CustomCommands.Features.SCPSwap.Commands
{
	[CommandHandler(typeof(SwapParentCommand))]
	public class Replace : ICustomCommand
	{
		public string Command => "replace";

		public string[] Aliases => null;
		public string Description => "Replaces you with an SCP who disconnected or swapped to human";

		public string[] Usage => null;

		public PlayerPermissions? Permission => null;

		public string PermissionString => string.Empty;

		public bool RequirePlayerSender => true;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (sender is PlayerCommandSender pSender && !pSender.ReferenceHub.IsSCP() && pSender.ReferenceHub.IsAlive())
			{
				var player = Player.Get(pSender.ReferenceHub);

				if (SCPSwap.CanHumanSwapToScp(player, out response))
				{
					SCPSwap.QueueSwapHumanToScp(player);

					response = "You have replaced an SCP";
					return true;
				}

				return false;
			}

			response = "You must be a living human role to run this command";
			return false;
		}
	}
}
