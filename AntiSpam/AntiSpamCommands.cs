using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using IrisLoader.Commands;
using MoreLinq;
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
					var settings = AntiSpamModule.Instance.GetSettings<AntiSpamSettingsModel>(ctx.Guild);
					bool canGrantRole = false;

					if (role != null)
					{
						if (canGrantRole = (await ctx.Guild.GetMemberAsync(ctx.Client.CurrentUser.Id)).Roles.Any(r => r.Position > role.Position))
						{
							settings.MuteRoleId = role.Id;
							AntiSpamModule.Instance.SetSettings(ctx.Guild, settings);

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
							embedBuilder = new ModernEmbedBuilder
							{
								Title = "Fehler",
								Color = 0xED4245,
								Fields =
								{
									("Details", $"Iris kann die Rolle {role.Mention} nicht vergeben, weil sie über der höchten Rolle des Bots liegt")
								}
							};
						}
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

					await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = !canGrantRole }.AddEmbed(embedBuilder.Build()));
				}
				[SlashRequireIrisPermission("ManageAntiSpam")]
				[SlashCommand("muteduration", "Die Zeit, für die Leute gemutet werden sollen")]
				public async Task MuteDurationCommand(InteractionContext ctx, [Option("weeks", "Wochen, für die die Mute-Rolle vergeben wird")] long weeks = 0, [Option("days", "Tage, für die die Mute-Rolle vergeben wird")] long days = 0, [Option("hours", "Stunden, für die die Mute-Rolle vergeben wird")] long hours = 0, [Option("minutes", "Minuten, für die die Mute-Rolle vergeben wird")] long minutes = 0)
				{
					ModernEmbedBuilder embedBuilder;
					var settings = AntiSpamModule.Instance.GetSettings<AntiSpamSettingsModel>(ctx.Guild);

					if (weeks + days + hours + minutes > 0)
					{
						ulong timeInMinutes = (ulong)weeks * 10080 + (ulong)days * 1440 + (ulong)hours * 60 + (ulong)minutes;

						settings.MuteDuration = timeInMinutes;
						AntiSpamModule.Instance.SetSettings(ctx.Guild, settings);

						embedBuilder = new ModernEmbedBuilder
						{
							Title = "Einstellung geändert",
							Color = 0x57F287,
							Fields =
							{
								("Details", $"`MuteDuration` auf `{TimeSpan.FromMinutes(timeInMinutes)}` gesetzt")
							}
						};
					}
					else if (weeks + days + hours + minutes < 0)
					{
						embedBuilder = new ModernEmbedBuilder
						{
							Title = "Fehler",
							Color = 0xED4245,
							Fields =
							{
								("Details", $"`MuteDuration` muss positiv sein")
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
								("Details", "`MuteDuration` ist momentan " + TimeSpan.FromMinutes(settings.MuteDuration))
							}
						};
					}

					await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = weeks + days + hours + minutes == 0 }.AddEmbed(embedBuilder.Build()));
				}
				[SlashRequireIrisPermission("ManageAntiSpam")]
				[SlashCommand("autodelete", "Schaltet das automatische Löschen von Spam an oder aus")]
				public async Task AutoDeleteCommand(InteractionContext ctx, [Option("value", "Ob Spam automatisch gelöscht werden sollen")] bool? value = null)
				{
					ModernEmbedBuilder embedBuilder;
					var settings = AntiSpamModule.Instance.GetSettings<AntiSpamSettingsModel>(ctx.Guild);

					if (value.HasValue)
					{
						settings.AutoDelete = value.Value;
						AntiSpamModule.Instance.SetSettings(ctx.Guild, settings);

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
					var settings = AntiSpamModule.Instance.GetSettings<AntiSpamSettingsModel>(ctx.Guild);

					if (value.HasValue)
					{
						settings.AutoMute = value.Value;
						AntiSpamModule.Instance.SetSettings(ctx.Guild, settings);

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
			var settings = AntiSpamModule.Instance.GetSettings<AntiSpamSettingsModel>(ctx.Guild);
			if (settings.MuteRoleId == 0)
			{
				embedBuilder = new ModernEmbedBuilder
				{
					Title = "Fehler",
					Color = 0xED4245,
					Fields =
					{
						("Details", "Für diesen Server wurde keine Mute-Rolle festgelegt")
					}
				};

				await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = true }.AddEmbed(embedBuilder.Build()));
				return;
			}

			_ = ctx.TargetMember.GrantRoleAsync(ctx.Guild.GetRole(settings.MuteRoleId));
			AntiSpamModule.Instance.AddReminder(TimeSpan.FromMinutes(settings.MuteDuration), 0, new string[] { ctx.TargetMember.Guild.Id.ToString(), ctx.TargetMember.Id.ToString() });

			embedBuilder = new ModernEmbedBuilder
			{
				Title = "User gemutet",
				Color = 0x57F287,
				Fields =
				{
					("Details", $"{ctx.User.Mention} hat {ctx.TargetUser.Mention} wegen Spam für {TimeSpan.FromMinutes(settings.MuteDuration)} gemuted")
				}
			};

			await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(embedBuilder.Build()));
		}
	}
}
