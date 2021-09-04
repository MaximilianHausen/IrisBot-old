using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using IrisLoader.Commands;
using IrisLoader.Modules.Global;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AntiSpam
{
	public class AntiSpamCommands : ApplicationCommandModule
	{
		[SlashCustomRequireGuild]
		[SlashRequireActiveModule(typeof(AntiSpamModule))]
		[SlashCommandGroup("antispam", "Commands für das Antispam-Modul")]
		public class AntiSpamCommandGroup
		{
			[SlashCommandGroup("settings", "Commands, um die Einstellungen zu verändern")]
			public class SettingsCommandGroup
			{
				[SlashRequireIrisPermission("ManageAntiSpam")]
				[SlashCommand("muterole", "Setzt die zum Stummschalten verwendete Rolle")]
				public async Task MuteRoleCommand(InteractionContext ctx, [Option("role", "Die Rolle, die zum muten verwendet werden soll")] DiscordRole role = null)
				{
					ModernEmbedBuilder embedBuilder;
					var settingsExt = AntiSpamModule.Instance.GetExtension<GlobalSettingsExtension>();
					var settings = settingsExt.GetSettings<AntiSpamSettingsModel>(ctx.Guild);

					if (role != null)
					{
						settings.MuteRoleId = role.Id;
						settingsExt.SetSettings(ctx.Guild, settings);

						embedBuilder = new ModernEmbedBuilder
						{
							Title = "Einstellung geändert",
							Color = 0x57F287,
							Fields =
							{
								("Details", $"`MuteRole` auf {role.Mention} gesetzt")
							}
						};
					}
					else
					{
						if (ctx.Guild.Roles.ContainsKey(settings.MuteRoleId))
						{
							embedBuilder = new ModernEmbedBuilder
							{
								Title = "Einstellung gelesen",
								Color = 0x57F287,
								Fields =
								{
									("Details", "Die momentane Mute-Rolle ist: " + ctx.Guild.GetRole(settings.MuteRoleId).Mention)
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
									("Details", "Für diesen Server wurde keine Mute-Rolle festgelegt")
								}
							};
						}
					}

					await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = role == null }.AddEmbed(embedBuilder.Build()));
				}
				[SlashRequireIrisPermission("ManageAntiSpam")]
				[SlashCommand("mutetime", "Die Zeit, für die Leute gemutet werden sollen")]
				public async Task MuteTimeCommand(InteractionContext ctx, [Option("weeks", "Wochen, für die die Mute-Rolle vergeben wird")] long weeks, [Option("days", "Tage, für die die Mute-Rolle vergeben wird")] long days, [Option("hours", "Stunden, für die die Mute-Rolle vergeben wird")] long hours, [Option("minutes", "Minuten, für die die Mute-Rolle vergeben wird")] long minutes)
				{
					long timeInMinutes = weeks * 10080 + days * 1440 + hours * 60 + minutes;
					TimeSpan timeSpan = new TimeSpan((int)timeInMinutes / 60, (int)timeInMinutes % 60, 0);
					var embedBuilder = new ModernEmbedBuilder
					{
						Title = "Einstellung geändert",
						Color = 0x57F287,
						Fields =
						{
							("Details", "Custom Mute-Times sind noch nicht verfügbar")
						}
					};
					await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = true }.AddEmbed(embedBuilder.Build()));
				}
				[SlashRequireIrisPermission("ManageAntiSpam")]
				[SlashCommand("autodelete", "Schaltet das automatische Löschen von Spam an oder aus")]
				public async Task AutoDeleteCommand(InteractionContext ctx, [Option("value", "Ob Spam automatisch gelöscht werden sollen")] bool? value = null)
				{
					ModernEmbedBuilder embedBuilder;
					var settingsExt = AntiSpamModule.Instance.GetExtension<GlobalSettingsExtension>();
					var settings = settingsExt.GetSettings<AntiSpamSettingsModel>(ctx.Guild);

					if (value.HasValue)
					{
						settings.AutoDelete = value.Value;
						settingsExt.SetSettings(ctx.Guild, settings);

						embedBuilder = new ModernEmbedBuilder
						{
							Title = "Einstellung geändert",
							Color = 0x57F287,
							Fields =
							{
								("Details", $"`AutoDelete` auf `{value}` gesetzt")
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
								("Details", "`Auto-Delete` ist momentan " + (settings.AutoDelete ? "aktiviert" : "deaktiviert"))
							}
						};
					}

					await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = !value.HasValue }.AddEmbed(embedBuilder.Build()));
				}
				[SlashRequireIrisPermission("ManageAntiSpam")]
				[SlashCommand("automute", "Schaltet das automatische Stummschalten von Spammern an oder aus")]
				public async Task AutoMuteCommand(InteractionContext ctx, [Option("value", "Ob Spammer automatisch gemutet werden sollen")] bool? value = null)
				{
					ModernEmbedBuilder embedBuilder;
					var settingsExt = AntiSpamModule.Instance.GetExtension<GlobalSettingsExtension>();
					var settings = settingsExt.GetSettings<AntiSpamSettingsModel>(ctx.Guild);

					if (value.HasValue)
					{
						settings.AutoMute = value.Value;
						settingsExt.SetSettings(ctx.Guild, settings);

						embedBuilder = new ModernEmbedBuilder
						{
							Title = "Einstellung geändert",
							Color = 0x57F287,
							Fields =
							{
								("Details", $"`Auto-Mute` auf {value} gesetzt")
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
								("Details", "`Auto-Mute` ist momentan " + (settings.AutoMute ? "aktiviert" : "deaktiviert"))
							}
						};
					}

					await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = !value.HasValue }.AddEmbed(embedBuilder.Build()));
				}
			}
		}

		[ContextMenuCustomRequireGuild]
		[ContextMenuRequireActiveModule(typeof(AntiSpamModule))]
		[ContextMenuRequireIrisPermission("Moderator")]
		[ContextMenu(ApplicationCommandType.UserContextMenu, "Mute")]
		public async Task MuteContextCommand(ContextMenuContext ctx)
		{
			ModernEmbedBuilder embedBuilder;
			var settingsExt = AntiSpamModule.Instance.GetExtension<GlobalSettingsExtension>();
			var settings = settingsExt.GetSettings<AntiSpamSettingsModel>(ctx.Guild);
			if (settings.MuteRoleId == 0)
			{
				embedBuilder = new ModernEmbedBuilder
				{
					Title = "Einstellung geändert",
					Color = 0x57F287,
					Fields =
					{
						("Details", "Für diesen Server wurde keine Mute-Rolle festgelegt")
					}
				};

				await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(embedBuilder.Build()));
				return;
			}

			DiscordRole muteRole = ctx.Guild.GetRole(settings.MuteRoleId);
			if (muteRole.Position > (await ctx.Guild.GetMemberAsync(AntiSpamModule.Instance.GetExtension<GlobalClientExtension>().Client.CurrentUser.Id)).Roles.Max(r => r.Position))
			{
				embedBuilder = new ModernEmbedBuilder
				{
					Title = "Einstellung geändert",
					Color = 0x57F287,
					Fields =
					{
						("Details", $"Iris kann {muteRole.Mention} nicht vergeben, weil sie in der Rangordnung höher ist als die des Bots")
					}
				};

				await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = true }.AddEmbed(embedBuilder.Build()));
				return;
			}
			_ = ctx.TargetMember.GrantRoleAsync(muteRole);

			embedBuilder = new ModernEmbedBuilder
			{
				Title = "Einstellung geändert",
				Color = 0x57F287,
				Fields =
				{
					("Details", $"{ctx.User.Mention} hat {ctx.TargetUser.Mention} wegen Spam für *momentan noch für immer* gemuted")
				}
			};

			await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(embedBuilder.Build()));
		}
	}
}
