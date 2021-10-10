using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using IrisLoader.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace AntiPing
{
	public class AntiPingCommands : ApplicationCommandModule
	{
		[SlashCustomRequireGuild]
		[SlashCommandGroup("antiping", "Commands für das Antiping-Modul")]
		public class AntiPingCommandGroup
		{
			[SlashCommandGroup("settings", "Commands, um die Einstellungen zu verändern")]
			public class SettingsCommandGroup
			{
				[SlashCommand("currentsettings", "Gibt eine Übersicht über alle Einstellungen von AntiPing")]
				public async Task SettingsCommand(InteractionContext ctx)
				{
					var settings = AntiPingModule.Instance.Connection.GetSettings<AntiPingSettingsModel>(ctx.Guild);

					var embedBuilder = new ModernEmbedBuilder
					{
						Title = "Einstellung gelesen",
						Color = 0x57F287,
						Fields =
					{
						("Details", $"`Auto-React`: {settings.AutoReact}\n" +
						$"`React-Emoji`: {(settings.ReactionEmoji == null ? "null" : (DiscordEmoji.IsValidUnicode(settings.ReactionEmoji) ? settings.ReactionEmoji : await ctx.Guild.GetEmojiAsync(ulong.Parse(settings.ReactionEmoji))))}\n\n" +
						$"`Ping-Back`: {settings.PingBack}\n" +
						$"`Ping-Time`: {settings.MinPingDelay}-{settings.MaxPingDelay} Minuten")
					}
					};

					await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = true }.AddEmbed(embedBuilder.Build()));
				}
				[SlashRequireIrisPermission("ManageAntiPing")]
				[SlashRequireActiveModule(typeof(AntiPingModule))]
				[SlashCommand("autoreact", "Schaltet das automatische Reagieren auf Antworten mit unnötigen Pings an oder aus")]
				public async Task AutoReactCommand(InteractionContext ctx, [Option("value", "Ob auf unnötige Pings automatisch reagiert werden soll")] bool? value = null)
				{
					ModernEmbedBuilder embedBuilder;
					var settings = AntiPingModule.Instance.Connection.GetSettings<AntiPingSettingsModel>(ctx.Guild);
					bool isEphemeral = true;

					if (value.HasValue && settings.ReactionEmoji != null)
					{
						isEphemeral = false;
						settings.AutoReact = value.Value;
						AntiPingModule.Instance.Connection.SetSettings(ctx.Guild, settings);

						embedBuilder = new ModernEmbedBuilder
						{
							Title = "Einstellung geändert",
							Color = 0x57F287,
							Fields =
							{
								("Details", $"`Auto-React` auf `{value}` gesetzt")
							}
						};
					}
					else if (value.HasValue)
					{
						embedBuilder = new ModernEmbedBuilder
						{
							Title = "Fehler",
							Color = 0xED4245,
							Fields =
							{
								("Details", $"Um `Auto-React` zu aktivieren, muss ein `React-Emoji` festgelegt werden")
							}
						};
					}
					else
					{
						embedBuilder = new ModernEmbedBuilder
						{
							Title = "Einstellung gelesen",
							Color = 0x57F287,
							Fields =
							{
								("Details", "`Auto-React` ist momentan " + (settings.AutoReact ? "aktiviert" : "deaktiviert"))
							}
						};
					}

					await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = isEphemeral }.AddEmbed(embedBuilder.Build()));
				}
				[SlashRequireIrisPermission("ManageAntiPing")]
				[SlashRequireActiveModule(typeof(AntiPingModule))]
				[SlashCommand("reactemoji", "Der Emoji, der für das automatische Reagieren verwendet wird")]
				public async Task ReactEmojiCommand(InteractionContext ctx, [Option("emoji", "Der Emoji, der für die automatische Reaktion verwendet wird")] string emoji = null)
				{
					string GetStringBetweenCharacters(string input, char charFrom, char charTo)
					{
						int posFrom = input.IndexOf(charFrom);
						if (posFrom != -1) //if found char
						{
							int posTo = input.IndexOf(charTo, posFrom + 1);
							if (posTo != -1) //if found char
							{
								return input.Substring(posFrom + 1, posTo - posFrom - 1);
							}
						}

						return string.Empty;
					}

					ModernEmbedBuilder embedBuilder;
					var settings = AntiPingModule.Instance.Connection.GetSettings<AntiPingSettingsModel>(ctx.Guild);
					bool isEphemeral = true;

					if (emoji != null && DiscordEmoji.IsValidUnicode(emoji))
					{
						isEphemeral = false;
						settings.ReactionEmoji = emoji;
						AntiPingModule.Instance.Connection.SetSettings(ctx.Guild, settings);

						embedBuilder = new ModernEmbedBuilder
						{
							Title = "Einstellung geändert",
							Color = 0x57F287,
							Fields =
							{
								("Details", $"`React-Emoji` auf {emoji} gesetzt")
							}
						};
					}
					else if (emoji != null && ctx.Guild.Emojis.Any(e => e.Value.Name == GetStringBetweenCharacters(emoji, ':', ':')))
					{
						isEphemeral = false;
						settings.ReactionEmoji = ctx.Guild.Emojis.First(e => e.Value.Name == GetStringBetweenCharacters(emoji, ':', ':')).Key.ToString();
						AntiPingModule.Instance.Connection.SetSettings(ctx.Guild, settings);

						embedBuilder = new ModernEmbedBuilder
						{
							Title = "Einstellung geändert",
							Color = 0x57F287,
							Fields =
							{
								("Details", $"`React-Emoji` auf {emoji} gesetzt")
							}
						};
					}
					else if (emoji != null)
					{
						embedBuilder = new ModernEmbedBuilder
						{
							Title = "Fehler",
							Color = 0xED4245,
							Fields =
							{
								("Details", $"`{emoji}` ist kein gültiger emoji")
							}
						};
					}
					else if (settings.ReactionEmoji != null)
					{
						embedBuilder = new ModernEmbedBuilder
						{
							Title = "Einstellung gelesen",
							Color = 0x57F287,
							Fields =
							{
								("Details", "`React-Emoji` ist momentan " + (DiscordEmoji.IsValidUnicode(settings.ReactionEmoji) ? settings.ReactionEmoji : await ctx.Guild.GetEmojiAsync(ulong.Parse(settings.ReactionEmoji))))
							}
						};
					}
					else
					{
						embedBuilder = new ModernEmbedBuilder
						{
							Title = "Einstellung gelesen",
							Color = 0x57F287,
							Fields =
							{
								("Details", "Momentan ist für diesen Server kein React-Emoji gesetzt")
							}
						};
					}

					await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = isEphemeral }.AddEmbed(embedBuilder.Build()));
				}
				[SlashRequireIrisPermission("ManageAntiPing")]
				[SlashRequireActiveModule(typeof(AntiPingModule))]
				[SlashCommand("pingback", "Schaltet das zurückpingen an oder aus")]
				public async Task PingBackCommand(InteractionContext ctx, [Option("value", "Ob bei unnötigen Pings nach einiger Zeit zurückgepingt werden soll")] bool? value = null)
				{
					ModernEmbedBuilder embedBuilder;
					var settings = AntiPingModule.Instance.Connection.GetSettings<AntiPingSettingsModel>(ctx.Guild);

					if (value.HasValue)
					{
						settings.PingBack = value.Value;
						AntiPingModule.Instance.Connection.SetSettings(ctx.Guild, settings);

						embedBuilder = new ModernEmbedBuilder
						{
							Title = "Einstellung geändert",
							Color = 0x57F287,
							Fields =
							{
								("Details", $"`Ping-Back` auf `{value}` gesetzt")
							}
						};
					}
					else
					{
						embedBuilder = new ModernEmbedBuilder
						{
							Title = "Einstellung gelesen",
							Color = 0x57F287,
							Fields =
							{
								("Details", "`Ping-Back` ist momentan " + (settings.PingBack ? "aktiviert" : "deaktiviert"))
							}
						};
					}

					await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = !value.HasValue }.AddEmbed(embedBuilder.Build()));
				}
				[SlashRequireIrisPermission("ManageAntiPing")]
				[SlashRequireActiveModule(typeof(AntiPingModule))]
				[SlashCommand("pingtime", "Setzt die Verzögerung, nach der zurückgepingt werden soll")]
				public async Task PingTimeCommand(InteractionContext ctx, [Option("min", "Minimale Zeit in Minuten")] long? min = null, [Option("max", "Maximale Zeit in Minuten")] long? max = null)
				{
					ModernEmbedBuilder embedBuilder;
					var settings = AntiPingModule.Instance.Connection.GetSettings<AntiPingSettingsModel>(ctx.Guild);
					bool isEphemeral = false;

					if ((min.HasValue && min < 0) || (max.HasValue && max < 0))
					{
						isEphemeral = true;
						embedBuilder = new ModernEmbedBuilder
						{
							Title = "Fehler",
							Color = 0xED4245,
							Fields =
							{
								("Details", $"`Ping-Time` muss positiv sein")
							}
						};
					}
					else if (min.HasValue && max.HasValue)
					{
						settings.MinPingDelay = min.Value;
						settings.MaxPingDelay = max.Value;
						AntiPingModule.Instance.Connection.SetSettings(ctx.Guild, settings);

						embedBuilder = new ModernEmbedBuilder
						{
							Title = "Einstellung geändert",
							Color = 0x57F287,
							Fields =
							{
								("Details", $"`Ping-Time` auf `{min}-{max} Minuten` gesetzt")
							}
						};
					}
					else if (min.HasValue)
					{
						settings.MinPingDelay = min.Value;
						AntiPingModule.Instance.Connection.SetSettings(ctx.Guild, settings);

						embedBuilder = new ModernEmbedBuilder
						{
							Title = "Einstellung geändert",
							Color = 0x57F287,
							Fields =
							{
								("Details", $"`Ping-Time` auf `{min}-{settings.MaxPingDelay} Minuten` gesetzt")
							}
						};
					}
					else if (max.HasValue)
					{
						settings.MinPingDelay = min.Value;
						AntiPingModule.Instance.Connection.SetSettings(ctx.Guild, settings);

						embedBuilder = new ModernEmbedBuilder
						{
							Title = "Einstellung geändert",
							Color = 0x57F287,
							Fields =
							{
								("Details", $"`Ping-Time` auf `{settings.MinPingDelay}-{max} Minuten` gesetzt")
							}
						};
					}
					else
					{
						isEphemeral = true;
						embedBuilder = new ModernEmbedBuilder
						{
							Title = "Einstellung gelesen",
							Color = 0x57F287,
							Fields =
							{
								("Details", $"`Ping-Time` ist momentan `{settings.MinPingDelay}-{settings.MaxPingDelay} Minuten`")
							}
						};
					}

					await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = isEphemeral }.AddEmbed(embedBuilder.Build()));
				}
			}
		}
	}
}
