using RedRightHand.CustomPlugin;
using System;

namespace ExternalQuery
{
	public class ExternalQueryPlugin : CustomPluginCore<Config>
	{
		public static SocketHandler Listener;

		public override string Name => "External Query";

		public override string Description => "Simple re-write of the base QueryProcessor system";

		public override Version Version => new(1, 1, 0);

		//Sorry spoobs. This one's all mine :3
		public override string Author => "PheWitch";

		public override string ConfigFileName => "ExternalQuery.yml";

		public override void Disable()
		{

		}

		public override void Enable()
		{
			Listener = new SocketHandler();
		}
	}
}
