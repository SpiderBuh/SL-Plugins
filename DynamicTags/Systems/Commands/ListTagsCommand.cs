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
	public class ListTagsCommand : ICommand
	{
		public string Command => "dynamictaglist";

		public string[] Aliases { get; } = { "dtaglist", "dtl" };

		public string Description => "Lists all dynamic tags";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (sender.CheckPermission(PlayerPermissions.PermissionsManagement))
			{
				List<string> tags = new List<string>();

				foreach (var tag in DynamicTags.Tags)
				{
					tags.Add($"{tag.Key} | {tag.Value.Tag}");
				}

				response = string.Join("\n", tags);
				return true;
			}
			response = "You cannot run this command";
			return true;
		}
	}
}
