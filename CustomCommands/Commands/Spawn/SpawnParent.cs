using CommandSystem;
using CommandSystem.Commands.RemoteAdmin.Cleanup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Commands.Spawn
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class SpawnParent : ParentCommand
	{
		public override string Command => "spawn";

		public override string[] Aliases => [];

		public override string Description => "Spawn various items";

		public override void LoadGeneratedCommands()
		{
			RegisterCommand(new Grenade());
			RegisterCommand(new Flashbang());
			RegisterCommand(new Ball());
		}

		public static SpawnParent Create()
		{
			var cleanupCommand = new SpawnParent();
			cleanupCommand.LoadGeneratedCommands();
			return cleanupCommand;
		}

		protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = "Please provide a valid subcommand. grenade/flash/ball";
			return false;
		}
	}
}
