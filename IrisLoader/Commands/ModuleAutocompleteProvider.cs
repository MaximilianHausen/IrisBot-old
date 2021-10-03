using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IrisLoader.Commands
{
	public class ModuleAutocompleteProvider : IAutocompleteProvider
	{
		public Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
		{
			if (!ctx.Interaction.GuildId.HasValue)
			{
				List<string> globalModules = Loader.GetGlobalModules().Where(m => m.Key.ToLower().Contains((ctx.OptionValue as string).ToLower())).Select(m => m.Key).ToList();
				if (!globalModules.Any()) return Task.FromResult(Array.Empty<DiscordAutoCompleteChoice>() as IEnumerable<DiscordAutoCompleteChoice>);
				return Task.FromResult(new List<DiscordAutoCompleteChoice>(globalModules.Select(m => new DiscordAutoCompleteChoice(m, m))) as IEnumerable<DiscordAutoCompleteChoice>);
			}

			List<string> modules = Loader.GetGlobalModules().Where(m => m.Key.ToLower().Contains((ctx.OptionValue as string).ToLower())).Select(m => m.Key).ToList();
			Loader.GetGuildModules(ctx.Interaction.Guild).Where(m => m.Key.ToLower().Contains((ctx.OptionValue as string).ToLower())).ForEach(m => modules.Add(m.Key));
			if (!modules.Any()) return Task.FromResult(Array.Empty<DiscordAutoCompleteChoice>() as IEnumerable<DiscordAutoCompleteChoice>);

			return Task.FromResult(new List<DiscordAutoCompleteChoice>(modules.Select(m => new DiscordAutoCompleteChoice(m, m))) as IEnumerable<DiscordAutoCompleteChoice>);
		}
	}
}
