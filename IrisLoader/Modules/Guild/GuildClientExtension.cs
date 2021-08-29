﻿using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using Emzi0767.Utilities;
using IrisLoader.Permissions;
using System.Threading.Tasks;

namespace IrisLoader.Modules.Guild
{
	public class GuildClientExtension : BaseIrisModuleExtension
	{
		internal GuildClientExtension()
		{
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
		~GuildClientExtension()
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

		public void RegisterCommands<T>() where T : ApplicationCommandModule
		{
			Loader.SlashExt.RegisterCommands<T>((Module as GuildIrisModule).Guild.Id);
			PermissionManager.RegisterPermissions<T>((Module as GuildIrisModule).Guild);
		}

		// Yes, I did this manually...
		private Task ChannelCreatedHandler(DiscordClient client, ChannelCreateEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? ChannelCreated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, ChannelCreateEventArgs> ChannelCreated;
		private Task ChannelDeletedHandler(DiscordClient client, ChannelDeleteEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? ChannelDeleted.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, ChannelDeleteEventArgs> ChannelDeleted;
		private Task ChannelPinsUpdatedHandler(DiscordClient client, ChannelPinsUpdateEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? ChannelPinsUpdated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, ChannelPinsUpdateEventArgs> ChannelPinsUpdated;
		private Task ChannelUpdatedHandler(DiscordClient client, ChannelUpdateEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? ChannelUpdated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, ChannelUpdateEventArgs> ChannelUpdated;
		private Task ComponentInteractionCreatedHandler(DiscordClient client, ComponentInteractionCreateEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? ComponentInteractionCreated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, ComponentInteractionCreateEventArgs> ComponentInteractionCreated;
		private Task ContextMenuInteractionCreatedHandler(DiscordClient client, ContextMenuInteractionCreateEventArgs e) => e.Interaction.GuildId == (Module as GuildIrisModule).Guild.Id ? ContextMenuInteractionCreated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, ContextMenuInteractionCreateEventArgs> ContextMenuInteractionCreated;
		private Task GuildBanAddedHandler(DiscordClient client, GuildBanAddEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? GuildBanAdded.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, GuildBanAddEventArgs> GuildBanAdded;
		private Task GuildBanRemovedHandler(DiscordClient client, GuildBanRemoveEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? GuildBanRemoved.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, GuildBanRemoveEventArgs> GuildBanRemoved;
		private Task GuildEmojisUpdatedHandler(DiscordClient client, GuildEmojisUpdateEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? GuildEmojisUpdated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, GuildEmojisUpdateEventArgs> GuildEmojisUpdated;
		private Task GuildIntegrationsUpdatedHandler(DiscordClient client, GuildIntegrationsUpdateEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? GuildIntegrationsUpdated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, GuildIntegrationsUpdateEventArgs> GuildIntegrationsUpdated;
		private Task GuildMemberAddedHandler(DiscordClient client, GuildMemberAddEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? GuildMemberAdded.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, GuildMemberAddEventArgs> GuildMemberAdded;
		private Task GuildMemberRemovedHandler(DiscordClient client, GuildMemberRemoveEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? GuildMemberRemoved.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, GuildMemberRemoveEventArgs> GuildMemberRemoved;
		private Task GuildMembersChunkedHandler(DiscordClient client, GuildMembersChunkEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? GuildMembersChunked.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, GuildMembersChunkEventArgs> GuildMembersChunked;
		private Task GuildMemberUpdatedHandler(DiscordClient client, GuildMemberUpdateEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? GuildMemberUpdated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, GuildMemberUpdateEventArgs> GuildMemberUpdated;
		private Task GuildRoleCreatedHandler(DiscordClient client, GuildRoleCreateEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? GuildRoleCreated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, GuildRoleCreateEventArgs> GuildRoleCreated;
		private Task GuildRoleDeletedHandler(DiscordClient client, GuildRoleDeleteEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? GuildRoleDeleted.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, GuildRoleDeleteEventArgs> GuildRoleDeleted;
		private Task GuildRoleUpdatedHandler(DiscordClient client, GuildRoleUpdateEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? GuildRoleUpdated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, GuildRoleUpdateEventArgs> GuildRoleUpdated;
		private Task GuildStickersUpdatedHandler(DiscordClient client, GuildStickersUpdateEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? GuildStickersUpdated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, GuildStickersUpdateEventArgs> GuildStickersUpdated;
		private Task GuildUpdatedHandler(DiscordClient client, GuildUpdateEventArgs e) => e.GuildAfter.Id == (Module as GuildIrisModule).Guild.Id ? GuildUpdated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, GuildUpdateEventArgs> GuildUpdated;
		private Task IntegrationCreatedHandler(DiscordClient client, IntegrationCreateEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? IntegrationCreated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, IntegrationCreateEventArgs> IntegrationCreated;
		private Task IntegrationDeletedHandler(DiscordClient client, IntegrationDeleteEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? IntegrationDeleted.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, IntegrationDeleteEventArgs> IntegrationDeleted;
		private Task IntegrationUpdatedHandler(DiscordClient client, IntegrationUpdateEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? IntegrationUpdated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, IntegrationUpdateEventArgs> IntegrationUpdated;
		private Task InteractionCreatedHandler(DiscordClient client, InteractionCreateEventArgs e) => e.Interaction.GuildId == (Module as GuildIrisModule).Guild.Id ? InteractionCreated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, InteractionCreateEventArgs> InteractionCreated;
		private Task InviteCreatedHandler(DiscordClient client, InviteCreateEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? InviteCreated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, InviteCreateEventArgs> InviteCreated;
		private Task InviteDeletedHandler(DiscordClient client, InviteDeleteEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? InviteDeleted.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, InviteDeleteEventArgs> InviteDeleted;
		private Task MessageAcknowledgedHandler(DiscordClient client, MessageAcknowledgeEventArgs e) => e.Channel.GuildId == (Module as GuildIrisModule).Guild.Id ? MessageAcknowledged.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, MessageAcknowledgeEventArgs> MessageAcknowledged;
		private Task MessageCreatedHandler(DiscordClient client, MessageCreateEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? MessageCreated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, MessageCreateEventArgs> MessageCreated;
		private Task MessageDeletedHandler(DiscordClient client, MessageDeleteEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? MessageDeleted.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, MessageDeleteEventArgs> MessageDeleted;
		private Task MessageReactionAddedHandler(DiscordClient client, MessageReactionAddEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? MessageReactionAdded.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, MessageReactionAddEventArgs> MessageReactionAdded;
		private Task MessageReactionRemovedHandler(DiscordClient client, MessageReactionRemoveEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? MessageReactionRemoved.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, MessageReactionRemoveEventArgs> MessageReactionRemoved;
		private Task MessageReactionRemovedEmojiHandler(DiscordClient client, MessageReactionRemoveEmojiEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? MessageReactionRemovedEmoji.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, MessageReactionRemoveEmojiEventArgs> MessageReactionRemovedEmoji;
		private Task MessageReactionsClearedHandler(DiscordClient client, MessageReactionsClearEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? MessageReactionsCleared.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, MessageReactionsClearEventArgs> MessageReactionsCleared;
		private Task MessagesBulkDeletedHandler(DiscordClient client, MessageBulkDeleteEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? MessagesBulkDeleted.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, MessageBulkDeleteEventArgs> MessagesBulkDeleted;
		private Task MessageUpdatedHandler(DiscordClient client, MessageUpdateEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? MessageUpdated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, MessageUpdateEventArgs> MessageUpdated;
		private Task PresenceUpdatedHandler(DiscordClient client, PresenceUpdateEventArgs e) => (Module as GuildIrisModule).Guild.Members.ContainsKey(e.User.Id) ? PresenceUpdated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, PresenceUpdateEventArgs> PresenceUpdated;
		private Task TypingStartedHandler(DiscordClient client, TypingStartEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? TypingStarted.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, TypingStartEventArgs> TypingStarted;
		private Task VoiceServerUpdatedHandler(DiscordClient client, VoiceServerUpdateEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? VoiceServerUpdated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, VoiceServerUpdateEventArgs> VoiceServerUpdated;
		private Task VoiceStateUpdatedHandler(DiscordClient client, VoiceStateUpdateEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? VoiceStateUpdated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, VoiceStateUpdateEventArgs> VoiceStateUpdated;
		private Task WebhooksUpdatedHandler(DiscordClient client, WebhooksUpdateEventArgs e) => e.Guild.Id == (Module as GuildIrisModule).Guild.Id ? WebhooksUpdated.Invoke(this, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildClientExtension, WebhooksUpdateEventArgs> WebhooksUpdated;
	}
}