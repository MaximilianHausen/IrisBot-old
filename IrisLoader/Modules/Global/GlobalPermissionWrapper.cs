﻿using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using IrisLoader.Permissions;
using System.Linq;

namespace IrisLoader.Modules.Global
{
	/// <summary> Only usable in global modules, will be checked before loading </summary>
	public static class GlobalPermissionWrapper
	{
		/// <returns> An array of all registered permissions for all guilds </returns>
		public static IrisPermission[] GetRegisteredPermissions() => (IrisPermission[])PermissionManager.GetRegisteredPermissions().Clone();
		/// <summary> You usually don't need to use this as it is executed automatically by registering commands </summary>
		public static void RegisterPermissions<T>(DiscordGuild guild) where T : ApplicationCommandModule => PermissionManager.RegisterPermissions<T>(guild);

		public static bool HasPermission(DiscordGuild guild, DiscordRole role, string permission) => PermissionManager.HasPermission(guild, role, permission);
		public static bool HasPermission(DiscordGuild guild, DiscordMember member, string permission) => member.Roles.Any(r => HasPermission(guild, r, permission));

		/// <summary> Sets a permission for a role </summary>
		/// <param name="role"> Tho role to modify the permission on </param>
		/// <param name="permission"> The permission to modify</param>
		/// <param name="value"> The value to set the permission to </param>
		public static void SetPermission(DiscordGuild guild, DiscordRole role, string permission, bool value) => PermissionManager.SetPermission(guild, role, permission, value);
		/// <returns> An array of all permissions given to this role </returns>
		public static string[] GetPermissions(DiscordGuild guild, DiscordRole role) => PermissionManager.GetPermissions(guild, role);
		/// <summary> Resets all permissions for this guild </summary>
		/// <param name="role"> Restrict to one role </param>
		public static void ResetPermissions(DiscordGuild guild, DiscordRole role = null) => PermissionManager.ResetPermissions(guild, role);
	}
}
