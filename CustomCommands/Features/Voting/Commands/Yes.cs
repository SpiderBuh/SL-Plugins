using CommandSystem;
using LabApi.Features.Wrappers;
using RemoteAdmin;
using System;

namespace CustomCommands.Features.Voting.Commands
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class Yes : ICommand
	{
		public string Command => "yes";

		public string[] Aliases => null;
		public string Description => "Vote yes on the current vote";

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (sender is PlayerCommandSender pSender)
			{
				if (!Votes.VoteInProgress)
				{
					response = "There is no vote in progress";
					return false;
				}

				var plr = Player.Get(pSender.ReferenceHub);

				if (Votes.PlayerVotes.ContainsKey(plr.UserId))
				{
					response = "You have already voted";
					return false;
				}

				Votes.PlayerVotes.Add(plr.UserId, true);

				response = "You have voted yes";
				return true;
			}

			response = "You must be a player to run this command";
			return false;
		}
	}
}
