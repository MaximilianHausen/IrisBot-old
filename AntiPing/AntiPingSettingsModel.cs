using DSharpPlus.Entities;
using IrisLoader.Settings;
using System.Text.Json.Serialization;

namespace AntiPing
{
	public class AntiPingSettingsModel
	{
		public bool Active { get; set; } = false;

		// Automatic reaction
		public bool AutoReact { get; set; } = false;
		[JsonConverter(typeof(EmojiJsonConverter))]
		public DiscordEmoji ReactionEmoji { get; set; } = null;

		// PingBack
		public bool PingBack { get; set; } = false;
		/// <summary> In minutes </summary>
		public long MinPingDelay { get; set; } = 30;
		/// <summary> In minutes </summary>
		public long MaxPingDelay { get; set; } = 2500;
	}
}
