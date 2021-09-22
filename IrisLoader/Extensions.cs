using DSharpPlus;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Linq;

namespace IrisLoader
{
	public static class Extensions
	{
		public static Dictionary<ulong, DiscordGuild> GetGuilds(this DiscordShardedClient client) => client.ShardClients.Values.SelectMany(c => c.Guilds).ToDictionary(g => g.Key, x => x.Value);
	}
}
