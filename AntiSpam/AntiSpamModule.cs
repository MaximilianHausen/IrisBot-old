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
        private readonly List<DiscordMessage> messageCache = new();
        public static AntiSpamModule Instance { get; private set; }
        public AntiSpamModule() => Instance = this;

        public override Task Load()
        {
            Connection.Client.MessageCreated += MessageCreated;
            Connection.Client.GuildRoleDeleted += RoleDeleted;
            Connection.Client.GuildRoleUpdated += GuildRoleUpdated;
            Connection.ReminderRecieved += ProcessReminder;
            Connection.RegisterCommands<AntiSpamCommands>();
            return Task.CompletedTask;
        }
        public override Task Unload()
        {
            Connection.Client.MessageCreated -= MessageCreated;
            Connection.Client.GuildRoleDeleted -= RoleDeleted;
            Connection.ReminderRecieved -= ProcessReminder;
            Connection.UnregisterCommands<AntiSpamCommands>();
            return Task.CompletedTask;
        }

        public override Task Ready()
        {
            Connection.UpdateSettignsFromFile<AntiSpamSettingsModel>();
            return Task.CompletedTask;
        }

        public override bool IsActive(DiscordGuild guild) => Connection.GetSettings<AntiSpamSettingsModel>(guild).Active;

        public override void SetActive(DiscordGuild guild, bool state)
        {
            AntiSpamSettingsModel settings = Connection.GetSettings<AntiSpamSettingsModel>(guild);
            settings.Active = state;
            Connection.SetSettings(guild, settings);
        }

        public async Task MessageCreated(DiscordClient client, MessageCreateEventArgs args)
        {
            if (args.Guild == null || args.Author.IsBot || !Connection.GetSettings<AntiSpamSettingsModel>(args.Guild).Active) return;
            messageCache.Add(args.Message);
            messageCache.RemoveAll(m => (DateTime.Now - m.CreationTimestamp) > TimeSpan.FromMinutes(5));
            List<DiscordMessage> countedMessages = messageCache.Where(m => (DateTime.Now - m.CreationTimestamp) < TimeSpan.FromSeconds(10)).Where(m => m.Author == args.Author).ToList();

            if (countedMessages.Count(m => m.Author == args.Author) > 5 || countedMessages.Select(m => m.Channel).Distinct().Count() > 2)
            {
                // Mute user
                List<Task<DiscordMember>> members = Connection.Client.GetGuilds().Select(g => g.Value.GetMemberAsync(args.Author.Id)).ToList();
                foreach (Task<DiscordMember> memberTask in members)
                {
                    DiscordMember member = await memberTask;
                    AntiSpamSettingsModel settings = Connection.GetSettings<AntiSpamSettingsModel>(member.Guild);
                    if (!settings.Active || !settings.AutoMute || settings.MuteRoleId == null) continue;

                    _ = MuteMemberAsync(member);
                    Connection.AddReminder(TimeSpan.FromMinutes(settings.MuteDuration), new string[] { member.Guild.Id.ToString(), member.Id.ToString() });
                }

                // Delete messages
                IEnumerable<DiscordChannel> channels = countedMessages.Select(m => m.Channel).Distinct();
                foreach (DiscordChannel channel in channels)
                {
                    AntiSpamSettingsModel settings = Connection.GetSettings<AntiSpamSettingsModel>(channel.Guild);
                    if (!settings.Active || !settings.AutoDelete) continue;

                    _ = channel.DeleteMessagesAsync(countedMessages.Where(m => m.Channel == channel));
                    messageCache.RemoveAll(m => m.Author == args.Author && m.Channel == channel && (DateTime.Now - m.CreationTimestamp) < TimeSpan.FromSeconds(10));
                }
            }
        }
        private async Task GuildRoleUpdated(DiscordClient client, GuildRoleUpdateEventArgs args)
        {
            AntiSpamSettingsModel settings = Connection.GetSettings<AntiSpamSettingsModel>(args.Guild);
            if (settings.MuteRoleId == null) return;

            if (!(await args.Guild.GetMemberAsync(client.CurrentUser.Id)).Roles.Any(r => r.Position > args.RoleAfter.Position))
            {
                settings.MuteRoleId = null;
                Connection.SetSettings(args.Guild, settings);
            }
        }
        public Task RoleDeleted(DiscordClient client, GuildRoleDeleteEventArgs args)
        {
            AntiSpamSettingsModel settings = Connection.GetSettings<AntiSpamSettingsModel>(args.Guild);
            if (settings.MuteRoleId == null) return Task.CompletedTask;

            settings.MuteRoleId = null;
            Connection.SetSettings(args.Guild, settings);

            return Task.CompletedTask;
        }
        public async Task ProcessReminder(BaseIrisModule sender, ReminderEventArgs args) => await UnmuteMemberAsync(await (await Connection.Client.GetShard(ulong.Parse(args.Values[0])).GetGuildAsync(ulong.Parse(args.Values[0]))).GetMemberAsync(ulong.Parse(args.Values[1])));

        public async Task MuteMemberAsync(DiscordMember member, bool respectAutoMuteSetting = false)
        {
            AntiSpamSettingsModel settings = Connection.GetSettings<AntiSpamSettingsModel>(member.Guild);
            if (!(settings.AutoMute || !respectAutoMuteSetting) || settings.MuteRoleId == null) return;

            await member.GrantRoleAsync(member.Guild.GetRole(settings.MuteRoleId.Value));
        }
        public async Task UnmuteMemberAsync(DiscordMember member)
        {
            AntiSpamSettingsModel settings = Connection.GetSettings<AntiSpamSettingsModel>(member.Guild);
            if (settings.MuteRoleId == null) return;

            await member.RevokeRoleAsync(member.Guild.GetRole(settings.MuteRoleId.Value));
        }
    }
}
