using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using IrisLoader.Commands;
using System;
using System.Threading.Tasks;

namespace AntiSpam
{
	public class AntiSpamCommands : ApplicationCommandModule
	{
		[SlashCommandGroup("antispam", "Commands für das Antispam-Modul")]
		public class AntiSpamCommandGroups
		{
			[SlashCustomRequireGuild]
			[SlashRequireIrisPermission("ManageAntiSpam")]
			[SlashCommand("setmuterole", "Setzt die zum Stummschalten verwendete Rolle")]
			public async Task MuteRoleCommand(InteractionContext ctx, [Option("role", "Die Rolle, die zum muten verwendet werden soll")] DiscordRole role)
			{

			}

			[SlashCustomRequireGuild]
			[SlashRequireIrisPermission("ManageAntiSpam")]
			[SlashCommand("mutetime", "Die Zeit, für die Leute gemutet werden sollen")]
			public async Task MuteTimeCommand(InteractionContext ctx, [Option("weeks", "Wochen, für die die Mute-Rolle vergeben wird")] int weeks, [Option("days", "Tage, für die die Mute-Rolle vergeben wird")] int days, [Option("hours", "Stunden, für die die Mute-Rolle vergeben wird")] int hours, [Option("minutes", "Minuten, für die die Mute-Rolle vergeben wird")] int minutes)
			{
				int timeInMinutes = weeks * 10080 + days * 1440 + hours * 60 + minutes;
				TimeSpan timeSpan = new TimeSpan(timeInMinutes / 60, timeInMinutes % 60, 0);
			}

			[SlashCommandGroup("autodetect", "Einstellungen für die automatische Erkennung von Spam")]
			public class AutoDetectCommandGroup
			{
				[SlashCustomRequireGuild]
				[SlashRequireIrisPermission("ManageAntiSpam")]
				[SlashCommand("autodelete", "Schaltet das automatische Löschen von Spam an oder aus")]
				public async Task AutoDeleteCommand(InteractionContext ctx, [Option("value", "Ob Spam automatisch gelöscht werden sollen")] bool value)
				{

				}
				[SlashCustomRequireGuild]
				[SlashRequireIrisPermission("ManageAntiSpam")]
				[SlashCommand("automute", "Schaltet das automatische Stummschalten von Spammern an oder aus")]
				public async Task AutoMuteCommand(InteractionContext ctx, [Option("value", "Ob Spammer automatisch gemutet werden sollen")] bool value)
				{

				}
			}
		}

		[ContextMenuRequireIrisPermission("ManageAntiSpam", true)]
		[ContextMenu(DSharpPlus.ApplicationCommandType.UserContextMenu, "mute")]
		public async Task MuteContextCommand(ContextMenuContext ctx)
		{

		}
	}
}
