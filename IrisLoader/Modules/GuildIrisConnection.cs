using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using Emzi0767.Utilities;
using IrisLoader.Permissions;
using System.Linq;
using System.Threading.Tasks;

namespace IrisLoader.Modules
{
	public sealed class GuildIrisConnection : BaseIrisConnection
	{
		internal GuildIrisConnection(GuildIrisModule module) : base(module)
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

		~GuildIrisConnection()
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

		#region Client
		public void RegisterCommands<T>() where T : ApplicationCommandModule
		{
			Loader.SlashExt.RegisterCommands<T>((module as GuildIrisModule).Guild.Id);
			PermissionManager.RegisterPermissions<T>((module as GuildIrisModule).Guild);
			if (Loader.IsConnected) Loader.SlashExt[0].RefreshCommands();
		}

		// Yes, I did this manually...
		private Task ChannelCreatedHandler(DiscordClient client, ChannelCreateEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? ChannelCreated.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, ChannelCreateEventArgs> ChannelCreated;
		private Task ChannelDeletedHandler(DiscordClient client, ChannelDeleteEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? ChannelDeleted.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, ChannelDeleteEventArgs> ChannelDeleted;
		private Task ChannelPinsUpdatedHandler(DiscordClient client, ChannelPinsUpdateEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? ChannelPinsUpdated.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, ChannelPinsUpdateEventArgs> ChannelPinsUpdated;
		private Task ChannelUpdatedHandler(DiscordClient client, ChannelUpdateEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? ChannelUpdated.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, ChannelUpdateEventArgs> ChannelUpdated;
		private Task ComponentInteractionCreatedHandler(DiscordClient client, ComponentInteractionCreateEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? ComponentInteractionCreated.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, ComponentInteractionCreateEventArgs> ComponentInteractionCreated;
		private Task ContextMenuInteractionCreatedHandler(DiscordClient client, ContextMenuInteractionCreateEventArgs e) => e.Interaction.GuildId == (module as GuildIrisModule).Guild.Id ? ContextMenuInteractionCreated.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, ContextMenuInteractionCreateEventArgs> ContextMenuInteractionCreated;
		private Task GuildBanAddedHandler(DiscordClient client, GuildBanAddEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? GuildBanAdded.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, GuildBanAddEventArgs> GuildBanAdded;
		private Task GuildBanRemovedHandler(DiscordClient client, GuildBanRemoveEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? GuildBanRemoved.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, GuildBanRemoveEventArgs> GuildBanRemoved;
		private Task GuildEmojisUpdatedHandler(DiscordClient client, GuildEmojisUpdateEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? GuildEmojisUpdated.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, GuildEmojisUpdateEventArgs> GuildEmojisUpdated;
		private Task GuildIntegrationsUpdatedHandler(DiscordClient client, GuildIntegrationsUpdateEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? GuildIntegrationsUpdated.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, GuildIntegrationsUpdateEventArgs> GuildIntegrationsUpdated;
		private Task GuildMemberAddedHandler(DiscordClient client, GuildMemberAddEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? GuildMemberAdded.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, GuildMemberAddEventArgs> GuildMemberAdded;
		private Task GuildMemberRemovedHandler(DiscordClient client, GuildMemberRemoveEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? GuildMemberRemoved.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, GuildMemberRemoveEventArgs> GuildMemberRemoved;
		private Task GuildMembersChunkedHandler(DiscordClient client, GuildMembersChunkEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? GuildMembersChunked.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, GuildMembersChunkEventArgs> GuildMembersChunked;
		private Task GuildMemberUpdatedHandler(DiscordClient client, GuildMemberUpdateEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? GuildMemberUpdated.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, GuildMemberUpdateEventArgs> GuildMemberUpdated;
		private Task GuildRoleCreatedHandler(DiscordClient client, GuildRoleCreateEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? GuildRoleCreated.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, GuildRoleCreateEventArgs> GuildRoleCreated;
		private Task GuildRoleDeletedHandler(DiscordClient client, GuildRoleDeleteEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? GuildRoleDeleted.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, GuildRoleDeleteEventArgs> GuildRoleDeleted;
		private Task GuildRoleUpdatedHandler(DiscordClient client, GuildRoleUpdateEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? GuildRoleUpdated.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, GuildRoleUpdateEventArgs> GuildRoleUpdated;
		private Task GuildStickersUpdatedHandler(DiscordClient client, GuildStickersUpdateEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? GuildStickersUpdated.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, GuildStickersUpdateEventArgs> GuildStickersUpdated;
		private Task GuildUpdatedHandler(DiscordClient client, GuildUpdateEventArgs e) => e.GuildAfter.Id == (module as GuildIrisModule).Guild.Id ? GuildUpdated.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, GuildUpdateEventArgs> GuildUpdated;
		private Task IntegrationCreatedHandler(DiscordClient client, IntegrationCreateEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? IntegrationCreated.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, IntegrationCreateEventArgs> IntegrationCreated;
		private Task IntegrationDeletedHandler(DiscordClient client, IntegrationDeleteEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? IntegrationDeleted.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, IntegrationDeleteEventArgs> IntegrationDeleted;
		private Task IntegrationUpdatedHandler(DiscordClient client, IntegrationUpdateEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? IntegrationUpdated.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, IntegrationUpdateEventArgs> IntegrationUpdated;
		private Task InteractionCreatedHandler(DiscordClient client, InteractionCreateEventArgs e) => e.Interaction.GuildId == (module as GuildIrisModule).Guild.Id ? InteractionCreated.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, InteractionCreateEventArgs> InteractionCreated;
		private Task InviteCreatedHandler(DiscordClient client, InviteCreateEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? InviteCreated.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, InviteCreateEventArgs> InviteCreated;
		private Task InviteDeletedHandler(DiscordClient client, InviteDeleteEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? InviteDeleted.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, InviteDeleteEventArgs> InviteDeleted;
		private Task MessageCreatedHandler(DiscordClient client, MessageCreateEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? MessageCreated.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, MessageCreateEventArgs> MessageCreated;
		private Task MessageDeletedHandler(DiscordClient client, MessageDeleteEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? MessageDeleted.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, MessageDeleteEventArgs> MessageDeleted;
		private Task MessageReactionAddedHandler(DiscordClient client, MessageReactionAddEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? MessageReactionAdded.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, MessageReactionAddEventArgs> MessageReactionAdded;
		private Task MessageReactionRemovedHandler(DiscordClient client, MessageReactionRemoveEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? MessageReactionRemoved.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, MessageReactionRemoveEventArgs> MessageReactionRemoved;
		private Task MessageReactionRemovedEmojiHandler(DiscordClient client, MessageReactionRemoveEmojiEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? MessageReactionRemovedEmoji.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, MessageReactionRemoveEmojiEventArgs> MessageReactionRemovedEmoji;
		private Task MessageReactionsClearedHandler(DiscordClient client, MessageReactionsClearEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? MessageReactionsCleared.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, MessageReactionsClearEventArgs> MessageReactionsCleared;
		private Task MessagesBulkDeletedHandler(DiscordClient client, MessageBulkDeleteEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? MessagesBulkDeleted.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, MessageBulkDeleteEventArgs> MessagesBulkDeleted;
		private Task MessageUpdatedHandler(DiscordClient client, MessageUpdateEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? MessageUpdated.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, MessageUpdateEventArgs> MessageUpdated;
		private Task PresenceUpdatedHandler(DiscordClient client, PresenceUpdateEventArgs e) => (module as GuildIrisModule).Guild.Members.ContainsKey(e.User.Id) ? PresenceUpdated.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, PresenceUpdateEventArgs> PresenceUpdated;
		private Task TypingStartedHandler(DiscordClient client, TypingStartEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? TypingStarted.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, TypingStartEventArgs> TypingStarted;
		private Task VoiceServerUpdatedHandler(DiscordClient client, VoiceServerUpdateEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? VoiceServerUpdated.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, VoiceServerUpdateEventArgs> VoiceServerUpdated;
		private Task VoiceStateUpdatedHandler(DiscordClient client, VoiceStateUpdateEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? VoiceStateUpdated.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, VoiceStateUpdateEventArgs> VoiceStateUpdated;
		private Task WebhooksUpdatedHandler(DiscordClient client, WebhooksUpdateEventArgs e) => e.Guild.Id == (module as GuildIrisModule).Guild.Id ? WebhooksUpdated.Invoke(module as GuildIrisModule, e) : Task.CompletedTask;
		public event AsyncEventHandler<GuildIrisModule, WebhooksUpdateEventArgs> WebhooksUpdated;
		#endregion

		#region IO
		public T ReadJson<T>(string relPath) => ModuleIO.ReadJson<T>((module as GuildIrisModule).Guild, module, relPath);
		public void WriteJson<T>(string relPath, T mapObject) => ModuleIO.WriteJson((module as GuildIrisModule).Guild, module, relPath, mapObject);
		#endregion

		#region Settings
		public T GetSettings<T>() where T : new() => ModuleSettings.GetSettings<T>((module as GuildIrisModule).Guild, module);
		public void SetSettings<T>(T settingsObject) => ModuleSettings.SetSettings((module as GuildIrisModule).Guild, module, settingsObject);

		public void UpdateFromFile<T>() where T : new() => ModuleSettings.UpdateFromFile<T>((module as GuildIrisModule).Guild, module);
		#endregion

		#region Permissions
		/// <returns> An array of all registered permissions for this guild </returns>
		public IrisPermission[] GetRegisteredPermissions() => PermissionManager.GetRegisteredPermissions().Where(p => p.guildId == (module as GuildIrisModule).Guild.Id || p.guildId == null).ToArray();
		/// <summary> You usually don't need to use this as it is executed automatically by registering commands </summary>
		public void RegisterPermissions<T>() where T : ApplicationCommandModule => PermissionManager.RegisterPermissions<T>((module as GuildIrisModule).Guild);

		public bool HasPermission(DiscordRole role, string permission) => PermissionManager.HasPermission((module as GuildIrisModule).Guild, role, permission);
		public bool HasPermission(DiscordMember member, string permission) => member.Roles.Any(r => HasPermission(r, permission));

		/// <summary> Sets a permission for a role </summary>
		/// <param name="role"> Tho role to modify the permission on </param>
		/// <param name="permission"> The permission to modify</param>
		/// <param name="value"> The value to set the permission to </param>
		public void SetPermission(DiscordRole role, string permission, bool value) => PermissionManager.SetPermission((module as GuildIrisModule).Guild, role, permission, value);
		/// <returns> An array of all permissions given to this role </returns>
		public string[] GetPermissions(DiscordRole role) => PermissionManager.GetPermissions((module as GuildIrisModule).Guild, role);
		/// <summary> Resets all permissions for this guild </summary>
		/// <param name="role"> Restrict to one role </param>
		public void ResetPermissions(DiscordRole role = null) => PermissionManager.ResetPermissions((module as GuildIrisModule).Guild, role);
		#endregion

		// Reminder inherited
	}
}
