using GameCore;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MEC;
using Newtonsoft.Json;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Extensions = RedRightHand.Extensions;

namespace DynamicTags.Systems
{
	public class StaffTracker : CustomEventsHandler
	{
		public override void OnPlayerJoined(PlayerJoinedEventArgs ev)
		{
			try
			{
				var details = new PlayerDetails
				{
					UserId = ev.Player.UserId,
					UserName = ev.Player.Nickname,
					Address = ev.Player.IpAddress,
					ServerAddress = Server.IpAddress,
					ServerPort = Server.Port.ToString()
				};

				Extensions.Post(DynamicTagsPlugin.Config.ApiUrl + "scpsl/playerjoin", new StringContent(JsonConvert.SerializeObject(details), Encoding.UTF8, "application/json"));
			}
			catch (Exception e)
			{
				Logger.Error($"Error during PlayerJoinedEvent: " + e.ToString());
			}
		}
		public override void OnPlayerLeft(PlayerLeftEventArgs ev)
		{
			try
			{
				var details = new PlayerDetails
				{
					UserId = ev.Player.UserId,
					UserName = ev.Player.Nickname,
					Address = ev.Player.IpAddress,
					ServerAddress = Server.IpAddress,
					ServerPort = Server.Port.ToString()
				};

				Extensions.Post(DynamicTagsPlugin.Config.ApiUrl + "scpsl/playerleave", new StringContent(JsonConvert.SerializeObject(details), Encoding.UTF8, "application/json"));

			}
			catch (Exception e)
			{
				Logger.Error($"Error during PlayerLeftEvent: " + e.ToString());
			}
		}
		public override void OnPlayerBanned(PlayerBannedEventArgs ev)
		{
			try
			{
				var details = new PlayerBanDetails
				{
					PlayerName = ev.Player.Nickname.Replace(':', ' '),
					PlayerID = ev.Player.UserId,
					PlayerAddress = ev.Player.IpAddress,
					AdminName = ev.Issuer.Nickname.Replace(':', ' '),
					AdminID = ev.Issuer.UserId,
					Duration = (ev.Duration / 60).ToString(),
					Reason = string.IsNullOrEmpty(ev.Reason) ? "No reason provided" : ev.Reason
				};

				Extensions.Post(DynamicTagsPlugin.Config.ApiUrl + "scpsl/playerban", new StringContent(JsonConvert.SerializeObject(details), Encoding.UTF8, "application/json"));

			}
			catch (Exception e)
			{
				Logger.Error($"Error during PlayerBannedEvent: " + e.ToString());
			}
		}
		public override void OnPlayerKicked(PlayerKickedEventArgs ev)
		{
			try
			{
				PlayerBanDetails details;

				if (!ev.Issuer.IsServer)
				{
					details = new PlayerBanDetails
					{
						PlayerName = ev.Player.Nickname.Replace(':', ' '),
						PlayerID = ev.Player.UserId,
						PlayerAddress = ev.Player.IpAddress,
						AdminName = ev.Issuer.Nickname.Replace(':', ' '),
						AdminID = ev.Issuer.UserId,
						Duration = "0",
						Reason = string.IsNullOrEmpty(ev.Reason) ? "No reason provided" : ev.Reason
					};
				}
				else
				{
					details = new PlayerBanDetails
					{
						PlayerName = ev.Player.Nickname.Replace(':', ' '),
						PlayerID = ev.Player.UserId,
						PlayerAddress = ev.Player.IpAddress,
						AdminName = "SERVER",
						AdminID = "server",
						Duration = "0",
						Reason = string.IsNullOrEmpty(ev.Reason) ? "No reason provided" : ev.Reason
					};

				}
				Extensions.Post(DynamicTagsPlugin.Config.ApiUrl + "scpsl/playerkick", new StringContent(JsonConvert.SerializeObject(details), Encoding.UTF8, "application/json"));

			}
			catch (Exception e)
			{
				Logger.Error($"Error during PlayerKickedEvent: " + e.ToString());
			}
		}
		public override void OnPlayerMuted(PlayerMutedEventArgs ev)
		{
			try
			{
				var details = new PlayerBanDetails
				{
					PlayerName = ev.Player.Nickname.Replace(':', ' '),
					PlayerID = ev.Player.UserId,
					PlayerAddress = ev.Player.IpAddress,
					AdminName = ev.Issuer.Nickname.Replace(':', ' '),
					AdminID = ev.Issuer.UserId,
					Duration = ev.IsIntercom.ToString(),
					Reason = "No reason provided"
				};

				Extensions.Post(DynamicTagsPlugin.Config.ApiUrl + "scpsl/playermute", new StringContent(JsonConvert.SerializeObject(details), Encoding.UTF8, "application/json"));

			}
			catch (Exception e)
			{
				Logger.Error($"Error during PlayerMutedEvent: " + e.ToString());
			}
		}
		public override void OnPlayerUnmuted(PlayerUnmutedEventArgs ev)
		{
			try
			{
				var details = new PlayerBanDetails
				{
					PlayerName = ev.Player.Nickname.Replace(':', ' '),
					PlayerID = ev.Player.UserId,
					PlayerAddress = ev.Player.IpAddress,
					AdminName = ev.Issuer.Nickname.Replace(':', ' '),
					AdminID = ev.Issuer.UserId,
					Duration = ev.IsIntercom.ToString(),
					Reason = "No reason provided"
				};

				Extensions.Post(DynamicTagsPlugin.Config.ApiUrl + "scpsl/playerunmute", new StringContent(JsonConvert.SerializeObject(details), Encoding.UTF8, "application/json"));

			}
			catch (Exception e)
			{
				Logger.Error($"Error during PlayerUnmutedEvent: " + e.ToString());
			}
		}

		public IEnumerator<float> CheckPreauth(PlayerPreAuthenticatingEventArgs args)
		{
			//try
			//{
			//	var details = new PlayerDetails
			//	{
			//		UserId = ev.UserId,
			//		Address = ev.IpAddress,
			//		ServerAddress = Server.ServerIpAddress,
			//		ServerPort = Server.Port.ToString()
			//	};

			//	var httpRM = Extensions.Post(DynamicTagsPlugin.Config.ApiEndpoint + "scpsl/playerpreauth", new StringContent(JsonConvert.SerializeObject(details), Encoding.UTF8, "application/json")).Result;
			//	var response = JsonConvert.DeserializeObject<APIResponse>(httpRM.Content.ReadAsStringAsync().Result);

			//	Logger.Info($"{response.Action} | {response.ReasonPlayer} | {response.ReasonPlayer}");

			//	ev.ConnectionRequest.RejectForce();
			//}
			//catch (Exception e)
			//{
			//	Log.Error($"Error during PlayerPreauthEvent: " + e.ToString());
			//}

			yield return 0f;
		}
	}
}
