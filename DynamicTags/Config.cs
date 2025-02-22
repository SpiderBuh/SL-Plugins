using RedRightHand.CustomPlugin;

namespace DynamicTags
{
	public class Config : CustomPluginConfig
	{
		/// <summary>
		/// Endpoint location for the external web API that the plugin will interact with.
		/// </summary>
		public string ApiUrl { get; set; } = "https://google.co.uk/";

		public bool TrackerEnabled { get; set; } = false;
		public bool TagsEnabled { get; set; } = true;
		public bool ReportingEnabled { get; set; } = false;
		public bool AutomaticNorthwoodReservedSlot { get; set; } = true;
	}
}
