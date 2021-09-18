namespace AntiSpam
{
	public class AntiSpamSettingsModel
	{
		public bool Active { get; set; } = true;
		public ulong MuteRoleId { get; set; } = 0;
		/// <summary> In minutes </summary>
		public ulong MuteDuration { get; set; } = 30;
		public bool AutoDelete { get; set; } = false;
		public bool AutoMute { get; set; } = true;
	}
}
