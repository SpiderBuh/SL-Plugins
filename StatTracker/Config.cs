using RedRightHand.CustomPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatTracker
{
	public class Config : CustomPluginConfig
	{
		public string ApiEndpoint { get; set; } = "https://google.co.uk/";
	}
}
