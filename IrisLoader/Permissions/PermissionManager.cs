﻿using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using IrisLoader.Commands;
using MoreLinq;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IrisLoader.Permissions
{
	internal static class PermissionManager
	{
		private static MySqlConnection con = new MySqlConnection();
		private static MySqlCommand cmd = new MySqlCommand();
		private static List<IrisPermission> permissions = new List<IrisPermission>();

		static PermissionManager()
		{
			string cs = "server=localhost;userid=root;password=" + Loader.config.MySqlPassword;

			con.ConnectionString = cs;
			cmd.Connection = con;
			con.Open();

			cmd.CommandText = "CREATE DATABASE IF NOT EXISTS iris_permissions";
			cmd.ExecuteNonQuery();
			cmd.CommandText = "USE iris_permissions";
			cmd.ExecuteNonQuery();

			cmd.CommandText = "DROP TABLE IF EXISTS iris_permissions";
			cmd.ExecuteNonQuery();

			cmd.CommandText = @"CREATE TABLE IF NOT EXISTS role_perms(id INTEGER UNSIGNED PRIMARY KEY AUTO_INCREMENT, guild_id BIGINT UNSIGNED, role_id BIGINT UNSIGNED, permission VARCHAR(32))";
			cmd.ExecuteNonQuery();
		}

		internal static IrisPermission[] GetRegisteredPermissions()
		{
			return permissions.ToArray();
		}
		private static void RegisterPermission(IrisPermission permission)
		{
			if (permissions.Where(p => !p.guildId.HasValue || p.guildId == permission.guildId).Select(p => p.name).Contains(permission.name))
				Logger.Log(Microsoft.Extensions.Logging.LogLevel.Error, 0, "Permissions", $"Permission registration{(permission.guildId.HasValue ? $" for guild \"{permission.guildId.Value}\"" : "")} failed: \"{permission.name}\" already registered");
			else
				permissions.Add(permission);
		}
		internal static void RegisterPermissions<T>(DiscordGuild guild) where T : ApplicationCommandModule => RegisterPermissions(typeof(T), guild?.Id);
		private static void RegisterPermissions(Type type, ulong? guildId)
		{
			List<IRequireIrisPermissionAttribute> attributes = new List<IRequireIrisPermissionAttribute>();
			type.GetMethods().ForEach(m => m.GetCustomAttributes<SlashRequireIrisPermissionAttribute>(true).ForEach(a => attributes.Add(a)));
			type.GetMethods().ForEach(m => m.GetCustomAttributes<ContextMenuRequireIrisPermissionAttribute>(true).ForEach(a => attributes.Add(a)));

			attributes.Select(a => a.Permission).Distinct().ForEach(p => RegisterPermission(new IrisPermission(p, guildId)));

			type.GetNestedTypes().ForEach(t => RegisterPermissions(t, guildId));
		}

		internal static bool HasPermission(DiscordGuild guild, DiscordRole role, string permission)
		{
			cmd.CommandText = $"SELECT * FROM role_perms WHERE guild_id = {guild.Id} AND role_id = {role.Id} AND permission = '{permission}'";
			using MySqlDataReader reader = cmd.ExecuteReader();
			return reader.HasRows;
		}
		internal static bool HasPermission(DiscordMember member, string permission) => member.Roles.Any(r => HasPermission(member.Guild, r, permission));

		internal static void SetPermission(DiscordGuild guild, DiscordRole role, string permission, bool value)
		{
			if (value && !HasPermission(guild, role, permission))
			{
				cmd.CommandText = $"INSERT INTO role_perms (guild_id, role_id, permission) VALUES ({guild.Id}, {role.Id}, '{permission}')";
				cmd.ExecuteNonQuery();
			}
			else if (!value && HasPermission(guild, role, permission))
			{
				cmd.CommandText = $"DELETE FROM role_perms WHERE guild_id = {guild.Id} AND role_id = {role.Id} AND permission = '{permission}'";
				cmd.ExecuteNonQuery();
			}
		}
		internal static string[] GetPermissions(DiscordGuild guild, DiscordRole role)
		{
			List<string> perms = new List<string>();
			cmd.CommandText = $"SELECT * FROM role_perms WHERE guild_id = {guild.Id} AND role_id = {role.Id}";
			using MySqlDataReader reader = cmd.ExecuteReader();

			while (reader.Read())
			{
				perms.Add(reader.GetString("permission"));
			}

			return perms.ToArray();
		}
		internal static void ResetPermissions(DiscordGuild guild, DiscordRole role = null)
		{
			if (role == null)
				cmd.CommandText = $"DELETE FROM role_perms WHERE guild_id = {guild.Id}";
			else
				cmd.CommandText = $"DELETE FROM role_perms WHERE guild_id = {guild.Id} AND role_id = {role.Id}";

			cmd.ExecuteNonQuery();
		}
	}
}
