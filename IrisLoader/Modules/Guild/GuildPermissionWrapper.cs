using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using IrisLoader.Permissions;
using System.Linq;

namespace IrisLoader.Modules.Guild
{
	// In GuildModules, use this to manage IrisPermissions
	public class GuildPermissionWrapper
	{
		private readonly DiscordGuild guild;
		public GuildPermissionWrapper(DiscordGuild guild) => this.guild = guild;

		/// <returns> An array of all registered permissions for this guild </returns>
		public IrisPermission[] GetRegisteredPermissions() => PermissionManager.GetRegisteredPermissions().Where(p => p.guildId == guild.Id || p.guildId == null).ToArray();
		/// <summary> You usually don't need to use this as it is executed automatically by registering commands </summary>
		public void RegisterPermissions<T>() where T : ApplicationCommandModule => PermissionManager.RegisterPermissions<T>(guild);

		public bool HasPermission(DiscordRole role, string permission) => PermissionManager.HasPermission(guild, role, permission);
		public bool HasPermission(DiscordMember member, string permission) => member.Roles.Any(r => HasPermission(r, permission));

		/// <summary> Sets a permission for a role </summary>
		/// <param name="role"> Tho role to modify the permission on </param>
		/// <param name="permission"> The permission to modify</param>
		/// <param name="value"> The value to set the permission to </param>
		public void SetPermission(DiscordRole role, string permission, bool value) => PermissionManager.SetPermission(guild, role, permission, value);
		/// <returns> An array of all permissions given to this role </returns>
		public string[] GetPermissions(DiscordRole role) => PermissionManager.GetPermissions(guild, role);
		/// <summary> Resets all permissions for this guild </summary>
		/// <param name="role"> Restrict to one role </param>
		public void ResetPermissions(DiscordRole role = null) => PermissionManager.ResetPermissions(guild, role);
	}
}
