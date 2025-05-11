using CommandSystem;
using LabApi.Features.Wrappers;
using RedRightHand;
using RedRightHand.Commands;
using System;

namespace CustomCommands.Features.SCPSwap.Commands
{
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class TriggerReplace : ICustomCommand
	{
		public string Command => "replacescp";

		public string[] Aliases => null;
		public string Description => "Manually triggers the SCP replacement broadcast";

		public string[] Usage => null;

		public PlayerPermissions? Permission => PlayerPermissions.ForceclassWithoutRestrictions;

		public string PermissionString => string.Empty;

		public bool RequirePlayerSender => false;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (Extensions.CanRun(sender, this, arguments, out response, out var _, out var plrSender))
				return false;

			if(Round.Duration > TimeSpan.FromSeconds(SCPSwap.SwapToScpSeconds))
			{
				response = $"You can only replace an SCP within the first {SCPSwap.SwapToScpSeconds} seconds of the round";
				return false;
			}

			SCPSwap.SCPsToReplace++;
			SCPSwap.ReplaceBroadcast();
			response = "SCP replace triggered";
			return true;
		}
	}
}
