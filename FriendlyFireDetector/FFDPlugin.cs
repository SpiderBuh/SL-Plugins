using LabApi.Events.CustomHandlers;
using RedRightHand.CustomPlugin;
using System;

namespace FriendlyFireDetector
{
    public class FFDPlugin : CustomPluginCore<Config>
    {
		public static Config Config;
		public static bool Paused = false;

		public override string ConfigFileName => "FFDConfig.yml";

		public override string Name => "Friendly Fire Detector";

		public override string Description => "Anti-Friendly Fire system";

		public override Version Version => new(1,1,0);

		public override void Enable()
		{
			Events = new Handler();
			CustomHandlersManager.RegisterEventsHandler(Events);
		}

		public override void Disable()
		{
			CustomHandlersManager.UnregisterEventsHandler(Events);
		}
	}
}
