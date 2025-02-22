using CommandSystem;
using LabApi.Features.Wrappers;
using RemoteAdmin;
using System;

namespace CustomCommands.Features.Voting.Commands
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class No : ICommand
	{
		public string Command => "no";
		public string[] Aliases => null;
		public string Description => "Vote no on the current vote";

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (sender is PlayerCommandSender pSender)
			{
				if (!Voting.VoteInProgress)
				{
					response = "There is no vote in progress";
					return false;
				}

				var plr = Player.Get(pSender.ReferenceHub);

				if (Voting.PlayerVotes.ContainsKey(plr.UserId))
				{
					response = "You have already voted";
					return false;
				}

				Voting.PlayerVotes.Add(plr.UserId, false);

				response = "You have voted no";
				return true;
			}

			response = "You must be a player to run this command";
			return false;
		}
	}
}
