using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using IrisLoader.Permissions;
using System.Linq;

namespace IrisLoader.Modules
{
	public abstract class GlobalIrisModule : BaseIrisModule
	{
		/// <returns> Whether the module is currently active or not </returns>
		public abstract bool IsActive(DiscordGuild guild);
		/// <summary> Activates/Deactivates a module </summary>
		/// <param name="state"> State to set the module to </param>
		public abstract void SetActive(DiscordGuild guild, bool state);

		#region Client
		// No abstraction needed here, global modules can only be uploaded by the developers
		public DiscordShardedClient Client
		{
			get { return Loader.Client; }
		}

		public void RegisterCommands<T>() where T : ApplicationCommandModule
		{
			Loader.SlashExt.RegisterCommands<T>();
			PermissionManager.RegisterPermissions<T>(null);
			if (Loader.IsConnected) Loader.SlashExt[0].RefreshCommands();
		}
		#endregion

		#region IO
		public T ReadJson<T>(DiscordGuild guild, string relPath) => ModuleIO.ReadJson<T>(guild, this, relPath);
		public void WriteJson<T>(DiscordGuild guild, string relPath, T mapObject) => ModuleIO.WriteJson(guild, this, relPath, mapObject);
		#endregion

		#region Settings
		public T GetSettings<T>(DiscordGuild guild) where T : new() => ModuleSettings.GetSettings<T>(guild, this);
		public void SetSettings<T>(DiscordGuild guild, T settingsObject) => ModuleSettings.SetSettings(guild, this, settingsObject);

		public void UpdateFromFile<T>() where T : new() => Loader.Client.GetGuilds().ForEach(g => ModuleSettings.UpdateFromFile<T>(g, this));
		#endregion

		#region Permissions
		/// <returns> An array of all registered permissions for all guilds </returns>
		public IrisPermission[] GetRegisteredPermissions() => (IrisPermission[])PermissionManager.GetRegisteredPermissions().Clone();
		/// <summary> You usually don't need to use this as it is executed automatically by registering commands </summary>
		public void RegisterPermissions<T>(DiscordGuild guild) where T : ApplicationCommandModule => PermissionManager.RegisterPermissions<T>(guild);

		public bool HasPermission(DiscordGuild guild, DiscordRole role, string permission) => PermissionManager.HasPermission(guild, role, permission);
		public bool HasPermission(DiscordGuild guild, DiscordMember member, string permission) => member.Roles.Any(r => HasPermission(guild, r, permission));

		/// <summary> Sets a permission for a role </summary>
		/// <param name="role"> Tho role to modify the permission on </param>
		/// <param name="permission"> The permission to modify</param>
		/// <param name="value"> The value to set the permission to </param>
		public void SetPermission(DiscordGuild guild, DiscordRole role, string permission, bool value) => PermissionManager.SetPermission(guild, role, permission, value);
		/// <returns> An array of all permissions given to this role </returns>
		public string[] GetPermissions(DiscordGuild guild, DiscordRole role) => PermissionManager.GetPermissions(guild, role);
		/// <summary> Resets all permissions for this guild </summary>
		/// <param name="role"> Restrict to one role </param>
		public void ResetPermissions(DiscordGuild guild, DiscordRole role = null) => PermissionManager.ResetPermissions(guild, role);
		#endregion

		// Reminder inherited
	}
}
