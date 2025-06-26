using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using Newtonsoft.Json;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DynamicTags.Systems
{
	public class CommandTracking : CustomEventsHandler
	{
		public override void OnServerCommandExecuting(CommandExecutingEventArgs ev)
		{
			if (Round.IsRoundStarted)
			{
				if (ev.Sender is PlayerCommandSender pCS && DynamicTagsPlugin.Config.CommandTrackingUserIds.Contains(pCS.ReferenceHub.authManager.UserId))
				{
					var plr = Player.Get(pCS);
					SendToDiscord($"User {plr.DisplayName} ({plr.UserId}) ran command: {ev.CommandName}" +
							$"\nPosition: {plr.Position} ({plr.Room}) | Camera: {plr.Camera.position} ({Room.GetRoomAtPosition(plr.Camera.position)})" +
							$"\nItems: {string.Join(", ", plr.Items.Select(i => i.Base.name))}" +
							$"\nLife: {plr.Role} | Noclip: {plr.IsNoclipEnabled} | Velocity: {plr.Velocity}");

					MEC.Timing.CallDelayed(1f, () =>
					{
						var plr = Player.Get(pCS);
						SendToDiscord($"**__DELAY CHECKUP:__**" +
							$"\nUser {plr.DisplayName} ({plr.UserId}) ran command: {ev.CommandName}" +
							$"\nPosition: {plr.Position} ({plr.Room}) | Camera: {plr.Camera.position} ({Room.GetRoomAtPosition(plr.Camera.position)})" +
							$"\nItems: {string.Join(", ", plr.Items.Select(i => i.Base.name))}" +
							$"\nLife: {plr.Role} | Noclip: {plr.IsNoclipEnabled} | Velocity: {plr.Velocity}");
					});
				}
			}
		}

		public void SendToDiscord(string content)
		{
			try
			{
				var webhook = new Webhook(content, "SL Command Tracker");

				RedRightHand.Extensions.Post(DynamicTagsPlugin.Config.CommandTrackingWebhook, new StringContent(JsonConvert.SerializeObject(webhook), Encoding.UTF8, "application/json"));
			}
			catch (Exception e)
			{
				Logger.Error($"Error during PlayerJoinedEvent: " + e.ToString());
			}
		}
	}

	public class Webhook
	{
		public string content;
		public string username;
		public string avatar_url;
		public bool tts = false;

		public Webhook(string content, string username)
		{
			this.content = content;
			this.username = username;
		}
	}
}
