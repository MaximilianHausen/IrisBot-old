using DSharpPlus;
using DSharpPlus.Entities;
using System.Collections.Generic;

namespace IrisLoader
{
	public static class Extensions
	{
		public static List<DiscordGuild> GetGuilds(this DiscordShardedClient client)
		{
			List<DiscordGuild> guilds = new List<DiscordGuild>();

			foreach (DiscordClient item in client.ShardClients.Values)
			{
				guilds.AddRange(item.Guilds.Values);
			}

			return guilds;
		}
	}
}
