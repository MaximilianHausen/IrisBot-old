using System;

namespace AntiSpam
{
	public class AntiSpamSettingsModel
	{
		public bool Active { get; set; } = true;
		public ulong MuteRoleId { get; set; } = 0;
		public TimeSpan MuteDuration { get; set; } = TimeSpan.FromMinutes(30); // Not yet used
		public bool AutoDelete { get; set; } = false;
		public bool AutoMute { get; set; } = true;
	}
}
