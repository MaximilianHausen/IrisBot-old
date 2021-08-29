using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using IrisLoader.Permissions;
using System.Linq;

namespace IrisLoader.Modules.Global
{
	/// <summary> This isn't static for consistency with its guild counterpart </summary>
	public class GlobalPermissionExtension : BaseIrisModuleExtension
	{
		internal GlobalPermissionExtension() { }

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
	}
}
