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
	public class StandartLoader : Loader
	{
		public StandartLoader(Config config) : base(config) { }

		private DiscordClient client;

		public override async Task MainAsync()
		{
			PermissionManager.Initialize(config.MySqlPassword);

			// Create client
			client = new DiscordClient(new DiscordConfiguration
			{
				Token = config.Token,
				TokenType = TokenType.Bot,
				Intents = DiscordIntents.AllUnprivileged,
				LogTimestampFormat = "d/M/yyyy hh:mm:ss",
				MinimumLogLevel = LogLevel.Information
			});

			client.UseInteractivity();
			var slash = client.UseSlashCommands();
			slash.RegisterCommands<LoaderCommands>();

			await LoadAllGlobalModulesAsync();
			// Register loader permissions
			PermissionManager.RegisterPermission(new IrisPermission("ManagePermissions", null));
			PermissionManager.RegisterPermission(new IrisPermission("ToggleModules", null));
			PermissionManager.RegisterPermission(new IrisPermission("ManageModules", null));

			// Register startup events
			client.GuildDownloadCompleted += Ready;

			await client.ConnectAsync();
			Console.ReadLine();
			await client.DisconnectAsync();
		}

		internal override DiscordClient GetClient(ulong? guildId) => client;
		internal override ILogger GetLogger() => client.Logger;

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
