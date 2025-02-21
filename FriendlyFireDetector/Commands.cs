using CommandSystem;
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
}
