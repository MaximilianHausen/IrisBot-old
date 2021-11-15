using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IrisLoader.Commands;

public class ModuleAutocompleteProvider : IAutocompleteProvider
{
    public Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
    {
        if (!ctx.Interaction.GuildId.HasValue)
        {
            List<string> globalModules = Loader.GetGlobalModules().Select(m => m.Key).Where(m => m.Contains(ctx.OptionValue as string, StringComparison.OrdinalIgnoreCase)).ToList();
            return !globalModules.Any()
                ? Task.FromResult(Array.Empty<DiscordAutoCompleteChoice>() as IEnumerable<DiscordAutoCompleteChoice>)
                : Task.FromResult(new List<DiscordAutoCompleteChoice>(globalModules.Select(m => new DiscordAutoCompleteChoice(m, m))) as IEnumerable<DiscordAutoCompleteChoice>);
        }

        List<string> modules = Loader.GetGlobalModules().Select(m => m.Key).Where(m => m.Contains(ctx.OptionValue as string, StringComparison.OrdinalIgnoreCase)).ToList();
        modules.AddRange(Loader.GetGuildModules(ctx.Interaction.Guild).Select(m => m.Key.ToLower()).Where(m => m.Contains(ctx.OptionValue as string)));
        return !modules.Any()
            ? Task.FromResult(Array.Empty<DiscordAutoCompleteChoice>() as IEnumerable<DiscordAutoCompleteChoice>)
            : Task.FromResult(new List<DiscordAutoCompleteChoice>(modules.Select(m => new DiscordAutoCompleteChoice(m, m))) as IEnumerable<DiscordAutoCompleteChoice>);
    }
}
