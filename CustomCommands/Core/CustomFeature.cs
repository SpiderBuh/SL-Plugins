using LabApi.Events.CustomHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Core
{
	public abstract class CustomFeature : CustomEventsHandler
	{
		public static CustomFeature Instance { get; private set; }
		public bool isEnabled { get; private set; }

		public CustomFeature(bool configSetting) {
			Instance = this;

			if (configSetting)
				OnEnabled();
		}

		public virtual void OnEnabled()
		{
			isEnabled = true;
			CustomHandlersManager.RegisterEventsHandler(this);
		}
		public virtual void OnDisabled()
		{
			isEnabled = false;
			CustomHandlersManager.UnregisterEventsHandler(this);
		}
	}
}
