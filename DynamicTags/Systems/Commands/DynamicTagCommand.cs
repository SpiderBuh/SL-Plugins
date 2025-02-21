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
	[CommandHandler(typeof(ClientCommandHandler))]
	public class DynamicTagCommand : ICommand
	{
		public string Command => "dynamictag";

		public string[] Aliases { get; } = { "dtag", "dt" };

		public string Description => "Shows your dynamic tag";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (sender is PlayerCommandSender pSender)
			{
				if (DynamicTags.Tags.ContainsKey(pSender.ReferenceHub.authManager.UserId))
				{
					TagData data = DynamicTags.Tags[pSender.ReferenceHub.authManager.UserId];

					//This is to stop situations where users have locally assigned perms but gets overridden by NULL perms from the external server.
					if (!string.IsNullOrEmpty(data.Group))
					{
						pSender.ReferenceHub.serverRoles.SetGroup(ServerStatic.PermissionsHandler.GetGroup(data.Group), true);
						pSender.ReferenceHub.serverRoles.RemoteAdmin = true;

						if (data.Perms != 0)
							pSender.ReferenceHub.serverRoles.Permissions = data.Perms;

					}

					pSender.ReferenceHub.serverRoles.SetText(data.Tag);
					pSender.ReferenceHub.serverRoles.SetColor(data.Colour);



					response = "Dynamic tag loaded: " + data.Tag;
					return true;
				}
				response = "You have no tag";
				return true;
			}

			response = "This command must be run as a player command";
			return false;
		}
	}
}
