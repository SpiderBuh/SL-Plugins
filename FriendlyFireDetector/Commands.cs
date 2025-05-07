using CommandSystem;
using RedRightHand;
using RedRightHand.Commands;
using RedRightHand.DataStores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendlyFireDetector
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class PauseCommand : ICommand
	{
		public string Command => "ffdpause";

		public string[] Aliases { get; } = { "ffdp" };

		public string Description => "Pause the Friendly Fire Detector";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			FFDPlugin.Paused = !FFDPlugin.Paused;

			response = $"Friendly Fire Detector is {(FFDPlugin.Paused ? "now" : "no longer")} paused";
			return true;
		}
	}

	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class ReverseCommand : ICustomCommand
	{
		public string Command => "ffdreverse";

		public string[] Aliases { get; } = { "ffr" };

		public string Description => "Pause the Friendly Fire Detector";

		public string[] Usage { get; } = ["%player%"];

		public string PermissionString => string.Empty;

		public bool RequirePlayerSender => false;

		public bool SanitizeResponse => false;

		public PlayerPermissions? Permission => null;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out var players, out _))
				return false;

			var ffdStore = players.First().GetDataStore<FFDStore>();
			ffdStore.ReverseFFEnabled = !ffdStore.ReverseFFEnabled;

			response = $"Friendly Fire Reversal is {(ffdStore.ReverseFFEnabled ? "now enabled" : "now disabled")} for {players.First().DisplayName}";
			return true;
		}
	}
}
