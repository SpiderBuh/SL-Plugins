using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Wrappers;
using Newtonsoft.Json;
using RedRightHand;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Extensions = RedRightHand.Extensions;


namespace DynamicTags.Systems
{
	public class Reporting : CustomEventsHandler
	{
		public static Dictionary<string, DateTime> ReportDict = new Dictionary<string, DateTime>();

		public override void OnPlayerReportingPlayer(PlayerReportingPlayerEventArgs ev)
		{
			if (ReportDict.TryGetValue(ev.Player.UserId, out var lastReport) && (DateTime.Now - lastReport).TotalMinutes < 2)
			{
				ev.IsAllowed = false;
			}
		}

		public override void OnPlayerReportedPlayer(PlayerReportedPlayerEventArgs ev)
		{
			var reportDetails = new PlayerReportDetails
			{
				PlayerName = ev.Target.Nickname,
				PlayerID = ev.Target.UserId,
				PlayerRole = ev.Target.Role.ToString(),
				PlayerAddress = ev.Target.IpAddress,
				ReporterName = ev.Player.Nickname,
				ReporterID = ev.Player.UserId,
				ReporterRole = ev.Player.Role.ToString(),
				Reason = ev.Reason,
				ServerAddress = Server.IpAddress,
				ServerPort = Server.Port.ToString(),
			};

			Extensions.Post(DynamicTagsPlugin.Config.ApiEndpoint + "scpsl/report", new StringContent(JsonConvert.SerializeObject(reportDetails), Encoding.UTF8, "application/json"));

			ReportDict.AddToOrReplaceValue(ev.Player.UserId, DateTime.Now);
		}
	}
}
