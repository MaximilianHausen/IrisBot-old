using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using IrisLoader;
using IrisLoader.Modules.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiSpam
{
	public class AntiSpamModule : GlobalIrisModule
	{
		private readonly List<DiscordMessage> messageCache = new List<DiscordMessage>();
		public static AntiSpamModule Instance { get; private set; }
		public AntiSpamModule() => Instance = this;

		public override bool IsActive(DiscordGuild guild)
		{
			return GetExtension<GlobalSettingsExtension>().GetSettings<AntiSpamSettingsModel>(guild).Active;
		}

		public override Task Load()
		{
			this.UseClient().Client.MessageCreated += MessageCreated;
			this.UseCommands().RegisterCommands<AntiSpamCommands>();
			this.UseSettings();
			return Task.CompletedTask;
		}
		public override Task Ready()
		{
			GetExtension<GlobalSettingsExtension>().UpdateFromFile<AntiSpamSettingsModel>();
			return Task.CompletedTask;
		}
		public override Task Unload()
		{
			return Task.CompletedTask;
		}

		public override void SetActive(DiscordGuild guild, bool state)
		{
			var settingsExt = GetExtension<GlobalSettingsExtension>();
			var settings = settingsExt.GetSettings<AntiSpamSettingsModel>(guild);
			settings.Active = state;
			settingsExt.SetSettings(guild, settings);
		}

		public async Task MessageCreated(DiscordClient client, MessageCreateEventArgs args)
		{
			if (args.Guild == null || !GetExtension<GlobalSettingsExtension>().GetSettings<AntiSpamSettingsModel>(args.Guild).Active) return;
			messageCache.Add(args.Message);
			messageCache.RemoveAll(m => (DateTime.Now - m.CreationTimestamp) > TimeSpan.FromMinutes(5));
			List<DiscordMessage> countedMessages = messageCache.Where(m => (DateTime.Now - m.CreationTimestamp) > TimeSpan.FromSeconds(10)).ToList();

			if (countedMessages.Count(m => m.Author == args.Author) > 5 || countedMessages.Select(m => m.Channel).Distinct().Count() > 2)
			{
				_ = MuteUser(args.Author, true);

				// Delete messages
				foreach (var channel in countedMessages.Where(m => m.Author == args.Author).Select(m => m.Channel).Distinct())
				{
					if (GetExtension<GlobalSettingsExtension>().GetSettings<AntiSpamSettingsModel>(channel.Guild).AutoDelete)
					{
						_ = channel.DeleteMessagesAsync(countedMessages.Where(m => m.Author == args.Author && m.Channel == channel));
						countedMessages.RemoveAll(m => m.Author == args.Author && m.Channel == channel);
					}
				}
				_ = UnmuteWithDelay(args.Author, TimeSpan.FromMinutes(5));
			}
		}

		public async Task MuteUser(DiscordUser user, bool respectAutoMuteSetting = false)
		{
			Dictionary<Task<DiscordMember>, DiscordRole> memberList = new();

			foreach (DiscordGuild guild in GetExtension<GlobalClientExtension>().Client.GetGuilds())
			{
				AntiSpamSettingsModel settings = GetExtension<GlobalSettingsExtension>().GetSettings<AntiSpamSettingsModel>(guild);
				if (settings.Active && (settings.AutoMute || !respectAutoMuteSetting) && settings.MuteRoleId != 0)
					memberList.Add(guild.GetMemberAsync(user.Id), guild.GetRole(settings.MuteRoleId));
			}

			foreach (var pair in memberList)
			{
				if (pair.Value != null)
					_ = (await pair.Key).GrantRoleAsync(pair.Value, "Spamming");
			}
		}
		public async Task UnmuteUser(DiscordUser user)
		{
			Dictionary<Task<DiscordMember>, DiscordRole> memberList = new();

			foreach (DiscordGuild guild in GetExtension<GlobalClientExtension>().Client.GetGuilds())
			{
				memberList.Add(guild.GetMemberAsync(user.Id), guild.GetRole(GetExtension<GlobalSettingsExtension>().GetSettings<AntiSpamSettingsModel>(guild).MuteRoleId));
			}

			foreach (var pair in memberList)
			{
				if (pair.Value != null)
					_ = (await pair.Key).RevokeRoleAsync(pair.Value);
			}
		}
		public async Task UnmuteWithDelay(DiscordUser user, TimeSpan delay)
		{
			await Task.Delay(delay);
			_ = UnmuteUser(user);
		}
	}
}
