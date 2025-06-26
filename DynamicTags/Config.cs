using RedRightHand.CustomPlugin;

namespace DynamicTags
{
	public class Config : CustomPluginConfig
	{
		/// <summary>
		/// Endpoint location for the external web API that the plugin will interact with.
		/// </summary>
		public string ApiUrl { get; set; } = "https://api.dragonscp.co.uk/";

		public bool TrackerEnabled { get; set; } = false;
		public bool TagsEnabled { get; set; } = true;
		public bool ReportingEnabled { get; set; } = false;
		public bool AutomaticNorthwoodReservedSlot { get; set; } = true;

		public string CommandTrackingWebhook { get; set; } = "MyAwesomeWebhookTokenHere";
		public string[] CommandTrackingUserIds { get; set; } = new string[] { };
	}
}
