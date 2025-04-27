using LabApi.Events.CustomHandlers;
using LabApi.Features.Console;
using PlayerRoles;
using RedRightHand.CustomPlugin;
using System;

namespace TutorialPlus
{
    public class TutorialPlusPlugin : CustomPluginCore<Config>
    {
        public static Config Config;

		public override string ConfigFileName => "TutorialConfig.yml";

		public override string Name => "Tutorial Plus";

		public override string Description => "Adds config options for tutorial";

		public override Version Version => new("1.1.0");

		public override void LoadConfigs()
		{
			base.LoadConfigs();
			Config = LoadPluginConfigs<Config>();
		}

		public override void Enable()
		{
			base.Enable();
			Events = new Events();

			CustomHandlersManager.RegisterEventsHandler(Events);
		}

		public override void Disable()
		{
			CustomHandlersManager.UnregisterEventsHandler(Events);
		}

		public static void DebugLog(string msg)
		{
			if (Config.Debug)
				Logger.Debug(msg);
		}
	}
}
