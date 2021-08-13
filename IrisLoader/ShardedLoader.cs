using DSharpPlus;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using IrisLoader.Commands;
using IrisLoader.Permissions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace IrisLoader
{
	public class ShardedLoader : Loader
	{
		public ShardedLoader(Config config) : base(config) { }

		private DiscordShardedClient client;

		public override async Task MainAsync()
		{
			PermissionManager.Initialize(config.MySqlPassword);

			// Create client
			client = new DiscordShardedClient(new DiscordConfiguration
			{
				Token = config.Token,
				TokenType = TokenType.Bot,
				Intents = DiscordIntents.AllUnprivileged,
				LogTimestampFormat = "d/M/yyyy hh:mm:ss",
				MinimumLogLevel = LogLevel.Information
			});

			await LoadAllGlobalModulesAsync();
			// Register loader permissions
			PermissionManager.RegisterPermission(new IrisPermission("ManagePermissions", null));
			PermissionManager.RegisterPermission(new IrisPermission("ToggleModules", null));
			PermissionManager.RegisterPermission(new IrisPermission("ManageModules", null));
			// Register global permissions
			//globalModules.Values.Select(m => m.module).ForEach(m => m.GetPermissions().ForEach(p => PermissionManager.RegisterPermission(new IrisPermission(p, null))));

			await client.UseInteractivityAsync();

			// Register commands
			var slash = await client.UseSlashCommandsAsync();
			slash.RegisterCommands<LoaderCommands>();

			// Register startup events
			client.GuildDownloadCompleted += Ready;

			await client.StartAsync();
			Console.ReadLine();
			await client.StopAsync();
		}

		public override DiscordClient GetClient(ulong? guildId) => guildId == null ? client.ShardClients[0] : client.GetShard(guildId.Value);
		public override ILogger GetLogger() => client.Logger;

		private Task Ready(DiscordClient client, DSharpPlus.EventArgs.GuildDownloadCompletedEventArgs args)
		{
			// List available guilds
			string guildList = "Iris is on the following Servers: ";
			foreach (var guild in client.Guilds.Values) { guildList += guild.Name + '@' + guild.Id + ", "; }
			guildList = guildList.Remove(guildList.Length - 2, 2);
			Logger.Log(Microsoft.Extensions.Logging.LogLevel.Information, 0, "Startup", guildList);
			return Task.CompletedTask;
		}
	}
}
