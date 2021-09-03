using DSharpPlus;
using DSharpPlus.Entities;
using IrisLoader.Modules.Global;
using IrisLoader.Modules.Guild;
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

		public static GlobalSettingsExtension UseSettings(this GlobalIrisModule module)
		{
			GlobalSettingsExtension ext = new GlobalSettingsExtension();
			module.AddExtension(ext);
			return ext;
		}
		public static GuildSettingsExtension UseSettings(this GuildIrisModule module)
		{
			GuildSettingsExtension ext = new GuildSettingsExtension();
			module.AddExtension(ext);
			return ext;
		}
		public static GlobalIOExtension UseIO(this GlobalIrisModule module)
		{
			GlobalIOExtension ext = new GlobalIOExtension();
			module.AddExtension(ext);
			return ext;
		}
		public static GuildIOExtension UseIO(this GuildIrisModule module)
		{
			GuildIOExtension ext = new GuildIOExtension();
			module.AddExtension(ext);
			return ext;
		}
		public static GlobalPermissionExtension UsePermissions(this GlobalIrisModule module)
		{
			GlobalPermissionExtension ext = new GlobalPermissionExtension();
			module.AddExtension(ext);
			return ext;
		}
		public static GuildPermissionExtension UsePermissions(this GuildIrisModule module)
		{
			GuildPermissionExtension ext = new GuildPermissionExtension();
			module.AddExtension(ext);
			return ext;
		}
		public static GlobalClientExtension UseClient(this GlobalIrisModule module)
		{
			GlobalClientExtension ext = new GlobalClientExtension();
			module.AddExtension(ext);
			return ext;
		}
		public static GuildClientExtension UseClient(this GuildIrisModule module)
		{
			GuildClientExtension ext = new GuildClientExtension();
			module.AddExtension(ext);
			return ext;
		}
		public static GlobalCommandExtension UseCommands(this GlobalIrisModule module)
		{
			GlobalCommandExtension ext = new GlobalCommandExtension();
			module.AddExtension(ext);
			return ext;
		}
		public static GuildCommandExtension UseCommands(this GuildIrisModule module)
		{
			GuildCommandExtension ext = new GuildCommandExtension();
			module.AddExtension(ext);
			return ext;
		}
	}
}
