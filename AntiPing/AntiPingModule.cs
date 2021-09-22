using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using IrisLoader.Modules;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AntiPing
{
	public class AntiPingModule : GlobalIrisModule
	{
		public static AntiPingModule Instance { get; private set; }
		public AntiPingModule() => Instance = this;

		public override Task Load()
		{
			Client.MessageCreated += MessageCreated;
			Client.GuildEmojisUpdated += EmojisEdited;
			ReminderRecieved += ProcessReminder;
			RegisterCommands<AntiPingCommands>();
			return Task.CompletedTask;
		}

		public override Task Unload()
		{
			Client.MessageCreated -= MessageCreated;
			ReminderRecieved -= ProcessReminder;
			return Task.CompletedTask;
		}

		public override Task Ready()
		{
			UpdateFromFile<AntiPingSettingsModel>();
			return Task.CompletedTask;
		}

		public override bool IsActive(DiscordGuild guild) => GetSettings<AntiPingSettingsModel>(guild).Active;
		public override void SetActive(DiscordGuild guild, bool state)
		{
			var settings = GetSettings<AntiPingSettingsModel>(guild);
			settings.Active = state;
			SetSettings(guild, settings);
		}

		public async Task MessageCreated(DiscordClient client, MessageCreateEventArgs args)
		{
			if (args.Guild == null || args.Author.IsBot || !GetSettings<AntiPingSettingsModel>(args.Guild).Active) return;

			var settings = GetSettings<AntiPingSettingsModel>(args.Guild);
			if (HasReplyPing(args.Message) && ((args.Message.Timestamp - args.Message.ReferencedMessage.Timestamp) < new TimeSpan(0, 30, 0)))
			{
				if (settings.AutoReact && settings.ReactionEmoji != null)
				{
					await args.Message.CreateReactionAsync(DiscordEmoji.IsValidUnicode(settings.ReactionEmoji) ? DiscordEmoji.FromUnicode(settings.ReactionEmoji) : await args.Guild.GetEmojiAsync(ulong.Parse(settings.ReactionEmoji)));
				}
				if (settings.PingBack)
				{
					AddReminder(TimeSpan.FromMinutes(new Random().Next((int)settings.MinPingDelay, (int)settings.MaxPingDelay)), new string[] { args.Guild.Id.ToString(), args.Channel.Id.ToString(), args.Author.Id.ToString() });
				}
			}
		}

		public async Task ProcessReminder(BaseIrisModule module, ReminderEventArgs args)
		{
			DiscordClient client = Client.GetShard(ulong.Parse(args.Values[0]));
			try
			{
				var channel = await client.GetChannelAsync(ulong.Parse(args.Values[1]));
				var member = await channel.Guild.GetMemberAsync(ulong.Parse(args.Values[2]));
				var pingMessage = await channel.SendMessageAsync(member.Mention);
				await pingMessage.DeleteAsync();
			}
			catch (Exception) { }
		}

		public Task EmojisEdited(DiscordClient client, GuildEmojisUpdateEventArgs args)
		{
			var settings = GetSettings<AntiPingSettingsModel>(args.Guild);
			if (!DiscordEmoji.IsValidUnicode(settings.ReactionEmoji) && !args.EmojisAfter.ContainsKey(ulong.Parse(settings.ReactionEmoji)))
			{
				settings.AutoReact = false;
				settings.ReactionEmoji = null;
				SetSettings(args.Guild, settings);
			}
			return Task.CompletedTask;
		}
		private bool HasReplyPing(DiscordMessage message)
		{
			if (message.ReferencedMessage == null) return false;
			return message.MentionedUsers.Contains(message.ReferencedMessage.Author);
		}
	}
}