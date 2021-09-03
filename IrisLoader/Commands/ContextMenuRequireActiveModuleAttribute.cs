using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using IrisLoader.Modules;
using IrisLoader.Modules.Global;
using IrisLoader.Modules.Guild;
using System;
using System.Threading.Tasks;

namespace IrisLoader.Commands
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class ContextMenuRequireActiveModuleAttribute : ContextMenuCheckBaseAttribute
	{
		private readonly BaseIrisModule requiredModule;
		public ContextMenuRequireActiveModuleAttribute(Type moduleType) => requiredModule = Loader.GetModuleByType(moduleType);
		public async override Task<bool> ExecuteChecksAsync(ContextMenuContext ctx)
		{
			if (requiredModule == null || ctx.Guild == null) return false;
			if (requiredModule is GlobalIrisModule module)
			{
				if (module.IsActive(ctx.Guild))
					return true;
				else
				{
					var embedBuilder = new ModernEmbedBuilder
					{
						Title = "Fehler",
						Color = 0xED4245,
						Fields =
						{
							("Details", $"Um diesen Command zu verwenden, muss das Modul `{requiredModule.Name}` aktiv sein")
						}
					};
					await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = true }.AddEmbed(embedBuilder.Build()));
					return false;
				}
			}
			else
			{
				if ((requiredModule as GuildIrisModule).IsActive())
					return true;
				else
				{
					var embedBuilder = new ModernEmbedBuilder
					{
						Title = "Fehler",
						Color = 0xED4245,
						Fields =
						{
							("Details", $"Um diesen Command zu verwenden, muss das Modul `{requiredModule.Name}` aktiv sein")
						}
					};
					await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = true }.AddEmbed(embedBuilder.Build()));
					return false;
				}
			}
		}
	}
}
