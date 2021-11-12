using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace IrisLoader
{
	public static class Extensions
	{
		public static Dictionary<ulong, DiscordGuild> GetGuilds(this DiscordShardedClient client) => client.ShardClients.Values.SelectMany(c => c.Guilds).ToDictionary(g => g.Key, x => x.Value);
		public static Task<DiscordGuild> GetGuildAsync(this DiscordShardedClient client, ulong guildId) => client.GetShard(guildId).GetGuildAsync(guildId);

		public static Task<DiscordChannel> AsClient(this DiscordChannel channel, DiscordClient client) => client.GetChannelAsync(channel.Id);

		public static void UnregisterCommands<T>(this IReadOnlyDictionary<int, SlashCommandsExtension> slashExt, ulong? guildId = null) => slashExt.Values.ForEach(e => e.UnregisterCommands<T>(guildId));
		public static void UnregisterCommands<T>(this SlashCommandsExtension slashExt, ulong? guildId = null) => (slashExt.GetType().GetProperty("_updateList", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(slashExt) as List<KeyValuePair<ulong?, Type>>).RemoveAll(p => p.Key == guildId && p.Value == typeof(T));

		public static Assembly GetAssembly(this object module) => Assembly.GetAssembly(module.GetType());
		public static AssemblyLoadContext GetAssemblyLoadContext(this object module) => AssemblyLoadContext.GetLoadContext(Assembly.GetAssembly(module.GetType()));
		public static string GetAssemblyPath(this object module) => module.GetType().Assembly.Location;
	}
}
