using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using System;
using System.Threading.Tasks;

namespace IrisLoader.Commands
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class SlashCustomRequireGuildAttribute : SlashCheckBaseAttribute
	{
		private readonly string message;
		public SlashCustomRequireGuildAttribute(string errormessage)
		{
			message = errormessage;
		}
		public SlashCustomRequireGuildAttribute()
		{
			message = null;
		}

		public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx)
		{
			if (ctx.Guild == null)
			{
				if (message != null)
				{
					var embedBuilder = new ModernEmbedBuilder
					{
						Title = "Fehler",
						Color = 0xED4245,
						Fields =
						{
							("Details", message)
						}
					};
					await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = true }.AddEmbed(embedBuilder.Build()));
					return false;
				}
				else
				{
					var embedBuilder = new ModernEmbedBuilder
					{
						Title = "Fehler",
						Color = 0xED4245,
						Fields =
						{
							("Details", "Dieser Command kann nur in einem Server verwendet werden")
						}
					};
					await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = true }.AddEmbed(embedBuilder.Build()));
					return false;
				}
			}
			else return true;
		}
	}
}
