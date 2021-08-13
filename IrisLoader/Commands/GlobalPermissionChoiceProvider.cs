using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using IrisLoader.Permissions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IrisLoader.Commands
{
	public class GlobalPermissionChoiceProvider : IChoiceProvider
	{
		public Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider()
		{
			return Task.FromResult(PermissionManager.GetRegisteredPermissions().Where(p => !p.guildId.HasValue).Select(s => new DiscordApplicationCommandOptionChoice(s.name, s.name)));
		}
	}
}
