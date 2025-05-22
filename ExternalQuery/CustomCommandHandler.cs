using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Extensions = RedRightHand.Extensions;

namespace ExternalQuery
{
	public class CustomCommandHandler
	{
		public static string BanCommand(string cmd)
		{
			try
			{
				string[] arg = cmd.Split(' ');

				string command = arg[0];
				string searchvariable = string.Empty;

				if (arg.Count() < 4 || arg[1].Length < 1)
					return $"{arg[0]} [UserID/IP] [Duration] [Reason]";

				//Forces the search variable to the front of the array
				arg = arg.Skip(1).ToArray();

				if (arg[0].StartsWith("'") || arg[0].StartsWith("\""))
				{
					string result = string.Join(" ", arg).Split(new string[] { "\"" }, 3, StringSplitOptions.None)[1];

					searchvariable = result;

					arg = string.Join(" ", arg).Replace("\"" + result + "\"", string.Empty).Trim(' ').Split(' ');
				}
				else
				{
					searchvariable = arg[0];

					arg = arg.Skip(1).ToArray();
				}

				var durationString = arg[0];
				if (!Extensions.TryGetBanDuration(durationString, out var duration))
					return "Invalid duration provided";

				var reason = string.Join(" ", arg.Skip(1).ToArray());

				if (Extensions.TryFindPlayer(searchvariable, out var player))
				{

					BanHandler.IssueBan(new BanDetails
					{
						OriginalName = player.Nickname,
						Id = player.UserId,
						Issuer = "SERVER (EXTERNAL QUERY)",
						IssuanceTime = DateTime.UtcNow.Ticks,
						Expires = DateTime.UtcNow.Add(duration).Ticks,
						Reason = reason
					}, BanHandler.BanType.UserId);

					BanHandler.IssueBan(new BanDetails
					{
						OriginalName = player.Nickname,
						Id = player.IpAddress,
						Issuer = "SERVER (EXTERNAL QUERY)",
						IssuanceTime = DateTime.UtcNow.Ticks,
						Expires = DateTime.UtcNow.Add(duration).Ticks,
						Reason = reason
					}, BanHandler.BanType.IP);

					player.Disconnect($"You have been banned by the server staff\nReason: " + reason);

					return $"{player.Nickname} ({player.UserId}) was banned for {durationString} with reason: {reason}";
				}
				else
				{
					BanHandler.IssueBan(new BanDetails
					{
						OriginalName = "Offline player",
						Id = searchvariable,
						Issuer = "SERVER (EXTERNAL QUERY)",
						IssuanceTime = DateTime.UtcNow.Ticks,
						Expires = DateTime.UtcNow.Add(duration).Ticks,
						Reason = reason
					}, searchvariable.Contains('@') ? BanHandler.BanType.UserId : BanHandler.BanType.IP);
					return $"{searchvariable} was banned for {durationString} with reason: {reason}!";
				}
			}
			catch (Exception e)
			{
				return e.ToString();
			}
		}
		public static string KickCommand(string cmd)
		{
			string[] arg = cmd.Split(' ');

			if (arg.Count() < 3 || arg[1].Length < 1)
				return $"{arg[0]} [UserID/IP] [Reason]";

			string searchvariable = string.Empty;

			//Forces the search variable to the front of the array
			arg = arg.Skip(1).ToArray();

			if (arg[0].StartsWith("'") || arg[0].StartsWith("\""))
			{
				string result = string.Join(" ", arg).Split(new string[] { "\"" }, 3, StringSplitOptions.None)[1];

				searchvariable = result;

				arg = string.Join(" ", arg).Replace("\"" + result + "\"", string.Empty).Trim(' ').Split(' ');
			}
			else
			{
				searchvariable = arg[0];

				arg = arg.Skip(1).ToArray();
			}

			if (!Extensions.TryFindPlayer(searchvariable, out var player))
				return $"Unable to find player {searchvariable.Replace("", "\\")}";

			string reason = string.Join(" ", arg);

			player.Disconnect($"You have been kicked by the server staff\nReason: " + reason);

			return $"{player.Nickname} ({player.UserId}) was kicked with reason: {reason}";
		}
		public static string UnbanCommand(string cmd)
		{
			string[] arg = cmd.Split(' ');

			if (arg.Count() < 2) return $"{arg[0]} [UserID/Ip]";

			bool validUID = arg[1].Contains('@');
			bool validIP = IPAddress.TryParse(arg[1], out IPAddress ip);

			BanDetails details;

			if (!validIP && !validUID)
				return $"Invalid UserID or IP given";

			if (validUID)
				details = BanHandler.QueryBan(arg[1], null).Key;
			else
				details = BanHandler.QueryBan(null, arg[1]).Value;

			if (details == null)
				return $"No ban found for {arg[1]}.\nMake sure you have typed it correctly, and that it has the @domain prefix if it's a UserID";

			BanHandler.RemoveBan(arg[1], (validUID ? BanHandler.BanType.UserId : BanHandler.BanType.IP));

			return $"{arg[1]} has been unbanned.";
		}
	}
}
