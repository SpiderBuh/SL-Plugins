using CommandSystem;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicTags.Systems.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class UpdateDynamicTagsCommand : ICommand
	{
		public string Command => "dynamictagupdate";

		public string[] Aliases { get; } = { "dtagupdate", "dtu" };

		public string Description => "Updates all dynamic tags";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (sender.CheckPermission(PlayerPermissions.PermissionsManagement))
			{
				DynamicTags.UpdateTags(true);

				response = "Dynamic tags updated";
				return true;
			}
			response = "You cannot run this command";
			return true;
		}
	}
}
