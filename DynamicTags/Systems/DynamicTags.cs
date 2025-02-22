using CommandSystem;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using Newtonsoft.Json;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using Extensions = RedRightHand.Extensions;

namespace DynamicTags.Systems
{
	public class DynamicTags : CustomEventsHandler
	{
		public static Dictionary<string, TagData> Tags = new Dictionary<string, TagData>();

		public override void OnServerWaitingForPlayers()
		{
			UpdateTags();
		}
		public static async void UpdateTags(bool ForceUpdate = false)
		{
			try
			{
				//Clears all previous tags held by the server (Prevents players from keeping tags when they have been removed from the external server).
				Tags.Clear();

				var response = await Extensions.Get(DynamicTagsPlugin.Config.ApiUrl + "games/gettags");

				var tags = JsonConvert.DeserializeObject<TagData[]>(await response.Content.ReadAsStringAsync());

				foreach (var a in tags)
				{
					if (a.UserID.StartsWith("7656"))
						a.UserID = $"{a.UserID}@steam";
					else if (ulong.TryParse(a.UserID, out ulong result))
						a.UserID = $"{a.UserID}@discord";
					else
						a.UserID = $"{a.UserID}@northwood";

					//Adds the tags to the tag list.
					Tags.Add(a.UserID, a);
				}

				Logger.Info($"{Tags.Count} tags loaded");

				foreach (var plr in Player.List)
					if (Tags.ContainsKey(plr.UserId))
						SetDynamicTag(plr, Tags[plr.UserId]);
			}
			catch (Exception e)
			{
				Logger.Error(e.ToString());
			}
		}

		public override void OnPlayerPreAuthenticating(PlayerPreAuthenticatingEventArgs ev)
		{
			if (ev.CanJoin)
				return;

			if(ev.UserId.ToLower().Contains("northwood") && DynamicTagsPlugin.Config.AutomaticNorthwoodReservedSlot)
			{
				Logger.Info($"Reserved slot bypass for {ev.UserId} (Northwood ID detected)");
				ev.CanJoin = true;
			}
			else if(Tags.TryGetValue(ev.UserId, out var tagData) && (tagData.ReservedSlot))
			{
				Logger.Info($"Reserved slot bypass for {ev.UserId} (Dynamic Tag)");
				ev.CanJoin = true;
			}
		}

		public override void OnPlayerJoined(PlayerJoinedEventArgs ev)
		{
			//Checks if the user has a tag
			if (Tags.ContainsKey(ev.Player.UserId))
				SetDynamicTag(ev.Player, Tags[ev.Player.UserId]);
		}

		public static void SetDynamicTag(Player player, TagData data)
		{
			//This is to stop situations where users have locally assigned perms but gets overridden by NULL perms from the external server.
			if (!string.IsNullOrEmpty(data.Group))
				player.ReferenceHub.serverRoles.SetGroup(ServerStatic.PermissionsHandler.GetGroup(data.Group), true);

			player.ReferenceHub.serverRoles.SetText(data.Tag);
			player.ReferenceHub.serverRoles.SetColor(data.Colour);

			if (data.Perms != 0)
				player.ReferenceHub.serverRoles.Permissions = data.Perms;

			player.SendConsoleMessage("Dynamic tag loaded: " + data.Tag);
			Logger.Info($"Tag found for {player.UserId}: {data.Tag}");
		}
	}
}
