using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using Emzi0767.Utilities;
using System.Threading.Tasks;

namespace IrisLoader.Modules.Guild
{
	/// <summary> Only usable in guild-modules, will be checked before loading </summary>
	public class GuildClientWrapper
	{
		private ulong guildId;

		public GuildClientWrapper(DiscordGuild guild)
		{
			guildId = guild.Id;
			DiscordShardedClient client = Loader.Client;
			client.ChannelCreated += ChannelCreatedHandler;
			client.ChannelDeleted += ChannelDeletedHandler;
			client.ChannelPinsUpdated += ChannelPinsUpdatedHandler;
			client.ChannelUpdated += ChannelUpdatedHandler;
			client.ComponentInteractionCreated += ComponentInteractionCreatedHandler;
			client.ContextMenuInteractionCreated += ContextMenuInteractionCreatedHandler;
			client.GuildBanAdded += GuildBanAddedHandler;
			client.GuildBanRemoved += GuildBanRemovedHandler;
			client.GuildEmojisUpdated += GuildEmojisUpdatedHandler;
			client.GuildIntegrationsUpdated += GuildIntegrationsUpdatedHandler;
			client.GuildMemberAdded += GuildMemberAddedHandler;
			client.GuildMemberRemoved += GuildMemberRemovedHandler;
			client.GuildMembersChunked += GuildMembersChunkedHandler;
			client.GuildMemberUpdated += GuildMemberUpdatedHandler;
			client.GuildRoleCreated += GuildRoleCreatedHandler;
			client.GuildRoleDeleted += GuildRoleDeletedHandler;
			client.GuildRoleUpdated += GuildRoleUpdatedHandler;
			client.GuildStickersUpdated += GuildStickersUpdatedHandler;
			client.GuildUpdated += GuildUpdatedHandler;
			client.IntegrationCreated += IntegrationCreatedHandler;
			client.IntegrationDeleted += IntegrationDeletedHandler;
			client.IntegrationUpdated += IntegrationUpdatedHandler;
			client.InteractionCreated += InteractionCreatedHandler;
			client.InviteCreated += InviteCreatedHandler;
			client.InviteDeleted += InviteDeletedHandler;
			client.MessageCreated += MessageCreatedHandler;
			client.MessageDeleted += MessageDeletedHandler;
			client.MessageReactionAdded += MessageReactionAddedHandler;
			client.MessageReactionRemoved += MessageReactionRemovedHandler;
			client.MessageReactionRemovedEmoji += MessageReactionRemovedEmojiHandler;
			client.MessageReactionsCleared += MessageReactionsClearedHandler;
			client.MessagesBulkDeleted += MessagesBulkDeletedHandler;
			client.MessageUpdated += MessageUpdatedHandler;
			client.PresenceUpdated += PresenceUpdatedHandler;
			client.TypingStarted += TypingStartedHandler;
			client.VoiceServerUpdated += VoiceServerUpdatedHandler;
			client.VoiceStateUpdated += VoiceStateUpdatedHandler;
			client.WebhooksUpdated += WebhooksUpdatedHandler;
		}
		~GuildClientWrapper()
		{
			DiscordShardedClient client = Loader.Client;
			client.ChannelCreated -= ChannelCreatedHandler;
			client.ChannelDeleted -= ChannelDeletedHandler;
			client.ChannelPinsUpdated -= ChannelPinsUpdatedHandler;
			client.ChannelUpdated -= ChannelUpdatedHandler;
			client.ComponentInteractionCreated -= ComponentInteractionCreatedHandler;
			client.ContextMenuInteractionCreated -= ContextMenuInteractionCreatedHandler;
			client.GuildBanAdded -= GuildBanAddedHandler;
			client.GuildBanRemoved -= GuildBanRemovedHandler;
			client.GuildEmojisUpdated -= GuildEmojisUpdatedHandler;
			client.GuildIntegrationsUpdated -= GuildIntegrationsUpdatedHandler;
			client.GuildMemberAdded -= GuildMemberAddedHandler;
			client.GuildMemberRemoved -= GuildMemberRemovedHandler;
			client.GuildMembersChunked -= GuildMembersChunkedHandler;
			client.GuildMemberUpdated -= GuildMemberUpdatedHandler;
			client.GuildRoleCreated -= GuildRoleCreatedHandler;
			client.GuildRoleDeleted -= GuildRoleDeletedHandler;
			client.GuildRoleUpdated -= GuildRoleUpdatedHandler;
			client.GuildStickersUpdated -= GuildStickersUpdatedHandler;
			client.GuildUpdated -= GuildUpdatedHandler;
			client.IntegrationCreated -= IntegrationCreatedHandler;
			client.IntegrationDeleted -= IntegrationDeletedHandler;
			client.IntegrationUpdated -= IntegrationUpdatedHandler;
			client.InteractionCreated -= InteractionCreatedHandler;
			client.InviteCreated -= InviteCreatedHandler;
			client.InviteDeleted -= InviteDeletedHandler;
			client.MessageCreated -= MessageCreatedHandler;
			client.MessageDeleted -= MessageDeletedHandler;
			client.MessageReactionAdded -= MessageReactionAddedHandler;
			client.MessageReactionRemoved -= MessageReactionRemovedHandler;
			client.MessageReactionRemovedEmoji -= MessageReactionRemovedEmojiHandler;
			client.MessageReactionsCleared -= MessageReactionsClearedHandler;
			client.MessagesBulkDeleted -= MessagesBulkDeletedHandler;
			client.MessageUpdated -= MessageUpdatedHandler;
			client.PresenceUpdated -= PresenceUpdatedHandler;
			client.TypingStarted -= TypingStartedHandler;
			client.VoiceServerUpdated -= VoiceServerUpdatedHandler;
			client.VoiceStateUpdated -= VoiceStateUpdatedHandler;
			client.WebhooksUpdated -= WebhooksUpdatedHandler;
		}

		public void RegisterCommands<T>() where T : ApplicationCommandModule => Loader.SlashExt.RegisterCommands<T>(guildId);

		// Yes, I did this manually...
		private Task ChannelCreatedHandler(DiscordClient client, ChannelCreateEventArgs e) => e.Guild.Id == guildId ? ChannelCreated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, ChannelCreateEventArgs> ChannelCreated;
		private Task ChannelDeletedHandler(DiscordClient client, ChannelDeleteEventArgs e) => e.Guild.Id == guildId ? ChannelDeleted.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, ChannelDeleteEventArgs> ChannelDeleted;
		private Task ChannelPinsUpdatedHandler(DiscordClient client, ChannelPinsUpdateEventArgs e) => e.Guild.Id == guildId ? ChannelPinsUpdated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, ChannelPinsUpdateEventArgs> ChannelPinsUpdated;
		private Task ChannelUpdatedHandler(DiscordClient client, ChannelUpdateEventArgs e) => e.Guild.Id == guildId ? ChannelUpdated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, ChannelUpdateEventArgs> ChannelUpdated;
		private Task ComponentInteractionCreatedHandler(DiscordClient client, ComponentInteractionCreateEventArgs e) => e.Guild.Id == guildId ? ComponentInteractionCreated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, ComponentInteractionCreateEventArgs> ComponentInteractionCreated;
		private Task ContextMenuInteractionCreatedHandler(DiscordClient client, ContextMenuInteractionCreateEventArgs e) => e.Interaction.GuildId == guildId ? ContextMenuInteractionCreated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, ContextMenuInteractionCreateEventArgs> ContextMenuInteractionCreated;
		private Task GuildBanAddedHandler(DiscordClient client, GuildBanAddEventArgs e) => e.Guild.Id == guildId ? GuildBanAdded.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, GuildBanAddEventArgs> GuildBanAdded;
		private Task GuildBanRemovedHandler(DiscordClient client, GuildBanRemoveEventArgs e) => e.Guild.Id == guildId ? GuildBanRemoved.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, GuildBanRemoveEventArgs> GuildBanRemoved;
		private Task GuildEmojisUpdatedHandler(DiscordClient client, GuildEmojisUpdateEventArgs e) => e.Guild.Id == guildId ? GuildEmojisUpdated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, GuildEmojisUpdateEventArgs> GuildEmojisUpdated;
		private Task GuildIntegrationsUpdatedHandler(DiscordClient client, GuildIntegrationsUpdateEventArgs e) => e.Guild.Id == guildId ? GuildIntegrationsUpdated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, GuildIntegrationsUpdateEventArgs> GuildIntegrationsUpdated;
		private Task GuildMemberAddedHandler(DiscordClient client, GuildMemberAddEventArgs e) => e.Guild.Id == guildId ? GuildMemberAdded.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, GuildMemberAddEventArgs> GuildMemberAdded;
		private Task GuildMemberRemovedHandler(DiscordClient client, GuildMemberRemoveEventArgs e) => e.Guild.Id == guildId ? GuildMemberRemoved.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, GuildMemberRemoveEventArgs> GuildMemberRemoved;
		private Task GuildMembersChunkedHandler(DiscordClient client, GuildMembersChunkEventArgs e) => e.Guild.Id == guildId ? GuildMembersChunked.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, GuildMembersChunkEventArgs> GuildMembersChunked;
		private Task GuildMemberUpdatedHandler(DiscordClient client, GuildMemberUpdateEventArgs e) => e.Guild.Id == guildId ? GuildMemberUpdated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, GuildMemberUpdateEventArgs> GuildMemberUpdated;
		private Task GuildRoleCreatedHandler(DiscordClient client, GuildRoleCreateEventArgs e) => e.Guild.Id == guildId ? GuildRoleCreated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, GuildRoleCreateEventArgs> GuildRoleCreated;
		private Task GuildRoleDeletedHandler(DiscordClient client, GuildRoleDeleteEventArgs e) => e.Guild.Id == guildId ? GuildRoleDeleted.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, GuildRoleDeleteEventArgs> GuildRoleDeleted;
		private Task GuildRoleUpdatedHandler(DiscordClient client, GuildRoleUpdateEventArgs e) => e.Guild.Id == guildId ? GuildRoleUpdated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, GuildRoleUpdateEventArgs> GuildRoleUpdated;
		private Task GuildStickersUpdatedHandler(DiscordClient client, GuildStickersUpdateEventArgs e) => e.Guild.Id == guildId ? GuildStickersUpdated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, GuildStickersUpdateEventArgs> GuildStickersUpdated;
		private Task GuildUpdatedHandler(DiscordClient client, GuildUpdateEventArgs e) => e.GuildAfter.Id == guildId ? GuildUpdated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, GuildUpdateEventArgs> GuildUpdated;
		private Task IntegrationCreatedHandler(DiscordClient client, IntegrationCreateEventArgs e) => e.Guild.Id == guildId ? IntegrationCreated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, IntegrationCreateEventArgs> IntegrationCreated;
		private Task IntegrationDeletedHandler(DiscordClient client, IntegrationDeleteEventArgs e) => e.Guild.Id == guildId ? IntegrationDeleted.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, IntegrationDeleteEventArgs> IntegrationDeleted;
		private Task IntegrationUpdatedHandler(DiscordClient client, IntegrationUpdateEventArgs e) => e.Guild.Id == guildId ? IntegrationUpdated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, IntegrationUpdateEventArgs> IntegrationUpdated;
		private Task InteractionCreatedHandler(DiscordClient client, InteractionCreateEventArgs e) => e.Interaction.GuildId == guildId ? InteractionCreated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, InteractionCreateEventArgs> InteractionCreated;
		private Task InviteCreatedHandler(DiscordClient client, InviteCreateEventArgs e) => e.Guild.Id == guildId ? InviteCreated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, InviteCreateEventArgs> InviteCreated;
		private Task InviteDeletedHandler(DiscordClient client, InviteDeleteEventArgs e) => e.Guild.Id == guildId ? InviteDeleted.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, InviteDeleteEventArgs> InviteDeleted;
		private Task MessageAcknowledgedHandler(DiscordClient client, MessageAcknowledgeEventArgs e) => e.Channel.GuildId == guildId ? MessageAcknowledged.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, MessageAcknowledgeEventArgs> MessageAcknowledged;
		private Task MessageCreatedHandler(DiscordClient client, MessageCreateEventArgs e) => e.Guild.Id == guildId ? MessageCreated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, MessageCreateEventArgs> MessageCreated;
		private Task MessageDeletedHandler(DiscordClient client, MessageDeleteEventArgs e) => e.Guild.Id == guildId ? MessageDeleted.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, MessageDeleteEventArgs> MessageDeleted;
		private Task MessageReactionAddedHandler(DiscordClient client, MessageReactionAddEventArgs e) => e.Guild.Id == guildId ? MessageReactionAdded.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, MessageReactionAddEventArgs> MessageReactionAdded;
		private Task MessageReactionRemovedHandler(DiscordClient client, MessageReactionRemoveEventArgs e) => e.Guild.Id == guildId ? MessageReactionRemoved.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, MessageReactionRemoveEventArgs> MessageReactionRemoved;
		private Task MessageReactionRemovedEmojiHandler(DiscordClient client, MessageReactionRemoveEmojiEventArgs e) => e.Guild.Id == guildId ? MessageReactionRemovedEmoji.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, MessageReactionRemoveEmojiEventArgs> MessageReactionRemovedEmoji;
		private Task MessageReactionsClearedHandler(DiscordClient client, MessageReactionsClearEventArgs e) => e.Guild.Id == guildId ? MessageReactionsCleared.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, MessageReactionsClearEventArgs> MessageReactionsCleared;
		private Task MessagesBulkDeletedHandler(DiscordClient client, MessageBulkDeleteEventArgs e) => e.Guild.Id == guildId ? MessagesBulkDeleted.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, MessageBulkDeleteEventArgs> MessagesBulkDeleted;
		private Task MessageUpdatedHandler(DiscordClient client, MessageUpdateEventArgs e) => e.Guild.Id == guildId ? MessageUpdated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, MessageUpdateEventArgs> MessageUpdated;
		private Task PresenceUpdatedHandler(DiscordClient client, PresenceUpdateEventArgs e) => Loader.Client.GetShard(guildId).GetGuildAsync(guildId).GetAwaiter().GetResult().Members.ContainsKey(e.User.Id) ? PresenceUpdated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, PresenceUpdateEventArgs> PresenceUpdated;
		private Task TypingStartedHandler(DiscordClient client, TypingStartEventArgs e) => e.Guild.Id == guildId ? TypingStarted.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, TypingStartEventArgs> TypingStarted;
		private Task VoiceServerUpdatedHandler(DiscordClient client, VoiceServerUpdateEventArgs e) => e.Guild.Id == guildId ? VoiceServerUpdated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, VoiceServerUpdateEventArgs> VoiceServerUpdated;
		private Task VoiceStateUpdatedHandler(DiscordClient client, VoiceStateUpdateEventArgs e) => e.Guild.Id == guildId ? VoiceStateUpdated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, VoiceStateUpdateEventArgs> VoiceStateUpdated;
		private Task WebhooksUpdatedHandler(DiscordClient client, WebhooksUpdateEventArgs e) => e.Guild.Id == guildId ? WebhooksUpdated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientWrapper, WebhooksUpdateEventArgs> WebhooksUpdated;
	}
}
