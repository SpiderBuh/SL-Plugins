using RedRightHand.CustomPlugin;

namespace ExternalQuery
{
	public class Config : CustomPluginConfig
	{
		public bool Enabled { get; set; } = false;
		public int Port { get; set; } = 8290;
		public string Password { get; set; } = "NEVERGONNAGIVEYOUUP";
	}
}
