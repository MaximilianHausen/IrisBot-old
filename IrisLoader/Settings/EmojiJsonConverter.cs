using DSharpPlus.Entities;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IrisLoader.Settings
{
	public class EmojiJsonConverter : JsonConverter<DiscordEmoji>
	{
		public override DiscordEmoji Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			ulong value = reader.GetUInt64();

			if (DiscordEmoji.IsValidUnicode(value.ToString()))
			{
				return DiscordEmoji.FromUnicode(value.ToString());
			}
			else
			{
				foreach (var guild in Loader.Client.GetGuilds().Values)
				{
					if (guild.Emojis.TryGetValue(value, out var found))
						return found;
				}
			}

			return null;
		}

		public override void Write(Utf8JsonWriter writer, DiscordEmoji value, JsonSerializerOptions options)
		{
			writer.WriteNumberValue(value.Id);
		}
	}
}
