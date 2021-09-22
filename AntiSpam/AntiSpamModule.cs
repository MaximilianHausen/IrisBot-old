using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using IrisLoader;
using IrisLoader.Modules;
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

		public override Task Load()
		{
			Client.MessageCreated += MessageCreated;
			Client.GuildRoleDeleted += RoleDeleted;
			Client.GuildRoleUpdated += GuildRoleUpdated;
			ReminderRecieved += ProcessReminder;
			RegisterCommands<AntiSpamCommands>();
			return Task.CompletedTask;
		}
		public override Task Unload()
		{
			Client.MessageCreated -= MessageCreated;
			Client.GuildRoleDeleted -= RoleDeleted;
			ReminderRecieved -= ProcessReminder;
			return Task.CompletedTask;
		}

		public override Task Ready()
		{
			UpdateFromFile<AntiSpamSettingsModel>();
			return Task.CompletedTask;
		}

		public override bool IsActive(DiscordGuild guild) => GetSettings<AntiSpamSettingsModel>(guild).Active;
		public override void SetActive(DiscordGuild guild, bool state)
		{
			var settings = GetSettings<AntiSpamSettingsModel>(guild);
			settings.Active = state;
			SetSettings(guild, settings);
		}

		public async Task MessageCreated(DiscordClient client, MessageCreateEventArgs args)
		{
			if (args.Guild == null || args.Author.IsBot || !GetSettings<AntiSpamSettingsModel>(args.Guild).Active) return;
			messageCache.Add(args.Message);
			messageCache.RemoveAll(m => (DateTime.Now - m.CreationTimestamp) > TimeSpan.FromMinutes(5));
			var countedMessages = messageCache.Where(m => (DateTime.Now - m.CreationTimestamp) < TimeSpan.FromSeconds(10)).Where(m => m.Author == args.Author).ToList();

			if (countedMessages.Count(m => m.Author == args.Author) > 5 || countedMessages.Select(m => m.Channel).Distinct().Count() > 2)
			{
				// Mute user
				List<Task<DiscordMember>> members = Client.GetGuilds().Select(g => g.Value.GetMemberAsync(args.Author.Id)).ToList();
				foreach (var memberTask in members)
				{
					var member = await memberTask;
					var settings = GetSettings<AntiSpamSettingsModel>(member.Guild);
					if (!settings.Active || !settings.AutoMute || settings.MuteRoleId == null) continue;

					_ = MuteMemberAsync(member);
					AddReminder(TimeSpan.FromMinutes(settings.MuteDuration), new string[] { member.Guild.Id.ToString(), member.Id.ToString() });
				}

				// Delete messages
				var channels = countedMessages.Select(m => m.Channel).Distinct();
				foreach (var channel in channels)
				{
					var settings = GetSettings<AntiSpamSettingsModel>(channel.Guild);
					if (!settings.Active || !settings.AutoDelete) continue;

					_ = channel.DeleteMessagesAsync(countedMessages.Where(m => m.Channel == channel));
					messageCache.RemoveAll(m => m.Author == args.Author && m.Channel == channel && (DateTime.Now - m.CreationTimestamp) < TimeSpan.FromSeconds(10));
				}
			}
		}
		private async Task GuildRoleUpdated(DiscordClient client, GuildRoleUpdateEventArgs args)
		{
			var settings = GetSettings<AntiSpamSettingsModel>(args.Guild);
			if (settings.MuteRoleId == null) return;

			if (!(await args.Guild.GetMemberAsync(client.CurrentUser.Id)).Roles.Any(r => r.Position > args.RoleAfter.Position))
			{
				settings.MuteRoleId = null;
				SetSettings(args.Guild, settings);
			}
		}
		public Task RoleDeleted(DiscordClient client, GuildRoleDeleteEventArgs args)
		{
			var settings = GetSettings<AntiSpamSettingsModel>(args.Guild);
			if (settings.MuteRoleId == null) return Task.CompletedTask;

			settings.MuteRoleId = null;
			SetSettings(args.Guild, settings);

			return Task.CompletedTask;
		}
		public async Task ProcessReminder(BaseIrisModule sender, ReminderEventArgs args)
		{
			await UnmuteMemberAsync(await (await Client.GetShard(ulong.Parse(args.Values[0])).GetGuildAsync(ulong.Parse(args.Values[0]))).GetMemberAsync(ulong.Parse(args.Values[1])));
		}


		public async Task MuteMemberAsync(DiscordMember member, bool respectAutoMuteSetting = false)
		{
			var settings = GetSettings<AntiSpamSettingsModel>(member.Guild);
			if (!(settings.AutoMute || !respectAutoMuteSetting) || settings.MuteRoleId == null) return;

			await member.GrantRoleAsync(member.Guild.GetRole(settings.MuteRoleId.Value));
		}
		public async Task UnmuteMemberAsync(DiscordMember member)
		{
			var settings = GetSettings<AntiSpamSettingsModel>(member.Guild);
			if (settings.MuteRoleId == null) return;

			await member.RevokeRoleAsync(member.Guild.GetRole(settings.MuteRoleId.Value));
		}
	}
}
