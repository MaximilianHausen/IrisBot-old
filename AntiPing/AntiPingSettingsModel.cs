namespace AntiPing
{
	public class AntiPingSettingsModel
	{
		public bool Active { get; set; } = false;

		// Automatic reaction
		public bool AutoReact { get; set; } = false;
		public string ReactionEmoji { get; set; } = null;

		// PingBack
		public bool PingBack { get; set; } = false;
		/// <summary> In minutes </summary>
		public long MinPingDelay { get; set; } = 30;
		/// <summary> In minutes </summary>
		public long MaxPingDelay { get; set; } = 2500;
	}
}
