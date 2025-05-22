using LabApi.Events.CustomHandlers;
using MEC;
using RedRightHand.CustomPlugin;
using System;
using System.Net;

namespace DiscordLab
{
	public class DiscordLabPlugin : CustomPluginCore<DiscordLabConfig>
	{
		public static DiscordLabConfig Config;
		public override string ConfigFileName => "DiscordLabConfig.yml";

		public override string Name => "DiscordLab";

		public override string Description => "Bridge between SCP:SL servers, and Discord";

		public override Version Version => new(1,1,0);

		public override void LoadConfigs()
		{
			base.LoadConfigs();
			Config = LoadPluginConfigs<DiscordLabConfig>();
		}

		public override void Disable()
		{
			CustomHandlersManager.UnregisterEventsHandler(Events);
		}

		public override void Enable()
		{
			new BotLink();
			Events = new Events();

			CustomHandlersManager.RegisterEventsHandler(Events);
		}
	}
}
