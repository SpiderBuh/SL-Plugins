using LabApi.Events.CustomHandlers;
using LabApi.Features;
using LabApi.Features.Console;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedRightHand.CustomPlugin
{
	public abstract class CustomPluginCore : Plugin
	{
		public override Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);
		public override string Author => "Dragon Inn Tech Team";
		public static CustomEventsHandler Events;
	}

	public abstract class CustomPluginCore<T> : CustomPluginCore where T : CustomPluginConfig
	{
		public static T Config { get; set; }
		private bool _correctConfigLoaded = false;
		public abstract string ConfigFileName { get; }

		public TConfig LoadPluginConfigs<TConfig>() where TConfig : class, new()
		{
			_correctConfigLoaded = this.TryLoadConfig(ConfigFileName, out TConfig loadedConfigs, false);
			return loadedConfigs;
		}

		public override void Enable()
		{
			if (!_correctConfigLoaded)
			{
				Logger.Error($"Configs have not loaded correctly. Please make sure the configs are correctly set. This plugin will NOT be enabled");
				return;
			}
		}
	}
}
