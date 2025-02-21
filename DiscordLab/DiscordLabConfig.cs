using RedRightHand.CustomPlugin;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordLab
{
	public class DiscordLabConfig : CustomPluginConfig
	{
		public int BotPort { get; set; }
		public string BotAddress { get; set; }
	}
}
