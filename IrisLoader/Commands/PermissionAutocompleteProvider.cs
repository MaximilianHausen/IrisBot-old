using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using IrisLoader.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IrisLoader.Commands
{
	public class PermissionAutocompleteProvider : IAutocompleteProvider
	{
		public Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
		{
			if (!ctx.Interaction.GuildId.HasValue)
			{
				IEnumerable<string> globalPermissions = PermissionManager.GetRegisteredPermissions().Where(p => !p.guildId.HasValue && p.name.ToLower().Contains((ctx.OptionValue as string).ToLower())).Select(p => p.name);
				if (!globalPermissions.Any()) return Task.FromResult(Array.Empty<DiscordAutoCompleteChoice>() as IEnumerable<DiscordAutoCompleteChoice>);
				return Task.FromResult(new List<DiscordAutoCompleteChoice>(globalPermissions.Select(p => new DiscordAutoCompleteChoice(p, p))) as IEnumerable<DiscordAutoCompleteChoice>);
			}

			IEnumerable<string> permissions = PermissionManager.GetRegisteredPermissions().Where(p => (p.guildId == ctx.Interaction.GuildId || p.guildId == null) && p.name.ToLower().Contains((ctx.OptionValue as string).ToLower())).Select(p => p.name);
			if (!permissions.Any()) return Task.FromResult(Array.Empty<DiscordAutoCompleteChoice>() as IEnumerable<DiscordAutoCompleteChoice>);
			return Task.FromResult(new List<DiscordAutoCompleteChoice>(permissions.Select(p => new DiscordAutoCompleteChoice(p, p))) as IEnumerable<DiscordAutoCompleteChoice>);
		}
	}
}
