using RedRightHand.CustomPlugin;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordLab
{
	public class DiscordLabConfig : CustomPluginConfig
	{
		public int BotPort { get; set; } = 8888;
		public string BotAddress { get; set; } = "127.0.0.1";
	}
}
