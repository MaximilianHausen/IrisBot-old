using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using IrisLoader.Commands;
using IrisLoader.Modules;
using IrisLoader.Permissions;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace IrisLoader;

public class LoaderCommands : ApplicationCommandModule
{
    [SlashCommandGroup("irisloader", "Commands für den Loader")]
    public class LoaderCommandGroup
    {
        [SlashCommandGroup("module", "Verwaltung von Modulen")]
        public class ModuleCommandGroup
        {
            [SlashCustomRequireGuild]
            [SlashCommand("list", "Liste aller verfügbaren Module")]
            public async Task ListCommand(InteractionContext ctx)
            {
                ModernEmbedBuilder embedBuilder;
                // name, active, true:global/false:guild
                List<(string, bool, bool)> moduleList = new();
                Loader.GetGlobalModules().ForEach(m => moduleList.Add((m.Key, m.Value.IsActive(ctx.Guild), true)));
                Loader.GetGuildModules(ctx.Guild).ForEach(m => moduleList.Add((m.Key, m.Value.IsActive(), false)));

                if (moduleList.Count > 0)
                {
                    string nameString = string.Join('\n', moduleList.Select(m => m.Item1));
                    string statusString = string.Join('\n', moduleList.Select(m => m.Item2 ? ":white_check_mark:" : ":x:"));
                    string scopeString = string.Join('\n', moduleList.Select(m => m.Item3 ? "global" : "lokal"));

                    embedBuilder = new ModernEmbedBuilder
                    {
                        Title = "Module list",
                        Color = 0xFEE75C,
                        Fields =
                        {
                            ("Modul", nameString, true),
                            ("Status", statusString, true),
                            ("Ursprung", scopeString, true)
                        }
                    };
                }
                else
                {
                    embedBuilder = new ModernEmbedBuilder
                    {
                        Title = "Module list",
                        Color = 0xFEE75C,
                        Fields =
                        {
                            ("Info", "Momentan sind keine Module geladen")
                        }
                    };
                }

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = true }.AddEmbed(embedBuilder.Build()));
            }

            [SlashCustomRequireGuild]
            [SlashRequireIrisPermission("ToggleModules")]
            [SlashCommand("toggle", "Ein Modul an-/ausschalten")]
            public async Task ToggleCommand(InteractionContext ctx, [Autocomplete(typeof(ModuleAutocompleteProvider))][Option("module", "Name des Moduls", true)] string moduleName)
            {
                ModernEmbedBuilder embedBuilder;
                bool prevState;
                GlobalIrisModule globalModule = Loader.GetGlobalModules().Where(m => m.Key == moduleName).SingleOrDefault().Value;
                GuildIrisModule guildModule = Loader.GetGuildModules(ctx.Guild).Where(m => m.Key == moduleName).SingleOrDefault().Value;

                if (globalModule != null)
                {
                    prevState = globalModule.IsActive(ctx.Guild);
                    globalModule.SetActive(ctx.Guild, !prevState);
                }
                else
                {
                    prevState = guildModule.IsActive();
                    guildModule.SetActive(!prevState);
                }

                embedBuilder = new ModernEmbedBuilder
                {
                    Title = prevState ? "Modul deaktiviert" : "Modul aktiviert",
                    Color = prevState ? 0xED4245 : 0x57F287,
                    Fields =
                    {
                        ("Details", "Das Modul " + moduleName + " ist jetzt " + (prevState ? "deaktiviert" : "aktiviert"))
                    }
                };

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(embedBuilder.Build()));
            }

            [SlashRequireIrisPermission("ManageModules")]
            [SlashCommand("upload", "Lädt ein neues Modul direkt von Discord")]
            public async Task UploadCommand(InteractionContext ctx)
            {
                bool loadAsGlobal = ctx.Guild == null;

                //Exit without permission
                if (loadAsGlobal && !ctx.Client.CurrentApplication.Owners.Any(x => x.Id == ctx.User.Id))
                {
                    ModernEmbedBuilder embedBuilder = new()
                    {
                        Title = "Fehler",
                        Color = 0xED4245,
                        Fields =
                        {
                            ("Details", "Nur die Entwickler des Bots (Maxi#2608) können globale Module verwalten")
                        }
                    };
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = true }.AddEmbed(embedBuilder.Build()));
                    return;
                }
                if (!loadAsGlobal && !PermissionManager.HasPermission(ctx.Member, "ManageModules") && !ctx.Member.Permissions.HasPermission(DSharpPlus.Permissions.Administrator))
                {
                    ModernEmbedBuilder embedBuilder = new()
                    {
                        Title = "Fehler",
                        Color = 0xED4245,
                        Fields =
                        {
                            ("Details", "Um diesen Command zu verwenden ist die Iris-Berechtigung `ManageModules` benötigt")
                        }
                    };
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = true }.AddEmbed(embedBuilder.Build()));
                    return;
                }

                if (!loadAsGlobal)
                {
                    ModernEmbedBuilder embedBuilder = new()
                    {
                        Title = "Fehler",
                        Color = 0xED4245,
                        Fields =
                        {
                            ("Details", "Momentan werden nur globale Module unterstützt")
                        }
                    };
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = true }.AddEmbed(embedBuilder.Build()));
                    return;
                }

                DiscordApplication app = ctx.Client.CurrentApplication;
                ModernEmbedBuilder responseBuilder = null;
                bool isThinking = false;

                // Wait for Message -> Download -> Load
                isThinking = true;
                await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
                bool condition(DiscordMessage m)
                {
                    bool rightUser = m.Author.Id == ctx.User.Id;
                    bool hasFile = m.Attachments.Count == 1;
                    bool isDll = hasFile && m.Attachments[0].FileName.EndsWith(".dll");
                    return rightUser && isDll;
                }
                InteractivityResult<DiscordMessage> fileInteraction = await ctx.Channel.GetNextMessageAsync(condition, new TimeSpan(0, 0, 30));

                if (fileInteraction.TimedOut)
                {
                    responseBuilder = new ModernEmbedBuilder
                    {
                        Title = "Fehler",
                        Color = 0xED4245,
                        Fields =
                        {
                            ("Details", "Timeout beim Warten auf die Upload-Nachricht")
                        }
                    };
                }
                else
                {
                    // Download Attachment
                    string fileUrl = fileInteraction.Result.Attachments[0].Url;
                    string cachePath = "./DownloadCache/" + fileInteraction.Result.Attachments[0].FileName;
                    Directory.CreateDirectory("./DownloadCache");
                    using (HttpClient httpClient = new())
                    {
                        await (await httpClient.GetStreamAsync(fileUrl)).CopyToAsync(new FileStream(cachePath, FileMode.Create));
                    }

                    // Check Attachment
                    bool isValid = await Loader.IsValidModule(cachePath, loadAsGlobal);

                    if (isValid)
                    {
                        // Move Module
                        string filePath = $"./Modules/{(loadAsGlobal ? "Global" : (ctx.Guild.Name + '~' + ctx.Guild.Id))}/" + fileInteraction.Result.Attachments[0].FileName;
                        Directory.CreateDirectory($"./Modules/{(loadAsGlobal ? "Global" : (ctx.Guild.Name + '~' + ctx.Guild.Id))}");
                        bool needsOverwrite = File.Exists(filePath);
                        if (needsOverwrite)
                            await Loader.UnloadGlobalModuleAsync(AssemblyName.GetAssemblyName(filePath).Name);
                        File.Move(cachePath, filePath, true);

                        // Load Module
                        string moduleName = AssemblyName.GetAssemblyName(filePath).Name;
                        bool success;
                        if (loadAsGlobal)
                        {
                            success = await Loader.LoadGlobalModuleAsync(moduleName);
                        }
                        else
                        {
#warning Load as guild
                            success = false;
                        }

                        if (success)
                        {
                            responseBuilder = new ModernEmbedBuilder
                            {
                                Title = "Modul geladen",
                                Color = 0x57F287,
                                Fields =
                                {
                                    ("Details", $"\"{moduleName}\" erfolgreich empfangen und als {(loadAsGlobal ? "globales" : "lokales")} Modul geladen")
                                }
                            };
                        }
                        else
                        {
                            responseBuilder = new ModernEmbedBuilder
                            {
                                Title = "Fehler",
                                Color = 0xED4245,
                                Fields =
                                {
                                    ("Details", $"\"{moduleName}\" konnte nicht als {(loadAsGlobal ? "globales" : "lokales")} Modul geladen werden")
                                }
                            };
                        }
                    }
                    else
                    {
                        // Ungültiges Modul
                        File.Delete(cachePath);
                        responseBuilder = new ModernEmbedBuilder
                        {
                            Title = "Fehler",
                            Color = 0xED4245,
                            Fields =
                            {
                                ("Details", "Die Datei `" + fileInteraction.Result.Attachments[0].FileName + "` enthält kein gültiges Modul. Bei weiteren Fragen an Maxi#2608 wenden")
                            }
                        };
                    }
                }

                if (responseBuilder != null)
                {
                    if (isThinking)
                    {
                        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(responseBuilder.Build()));
                    }
                    else
                    {
                        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = true }.AddEmbed(responseBuilder.Build()));
                    }
                }
            }

            [SlashRequireIrisPermission("ManageModules")]
            [SlashCommand("delete", "Löscht ein Modul")]
            public async Task DeleteCommand(InteractionContext ctx, [Autocomplete(typeof(ModuleAutocompleteProvider))][Option("name", "Name des zu löschenden Moduls", true)] string name)
            {
                bool usedByOwner = ctx.Client.CurrentApplication.Owners.Any(x => x.Id == ctx.User.Id);
                GlobalIrisModule globalModule = Loader.GetGlobalModules().FirstOrDefault(m => m.Key == name).Value;
                GuildIrisModule guildModule = Loader.GetGuildModules(ctx.Guild).FirstOrDefault(m => m.Key == name).Value;

                if (globalModule != null)
                {
                    if (usedByOwner)
                    {
                        if (await Loader.UnloadGlobalModuleAsync(name))
                        {
                            File.Delete(globalModule.GetAssemblyPath());
                            // Loader.Client.GetGuilds().ForEach(g => Directory.Delete(ModuleIO.GetModuleFileDirectory(g.Value, name).FullName));
                            ModernEmbedBuilder embedBuilder = new()
                            {
                                Title = "Modul gelöscht",
                                Color = 0x57F287,
                                Fields =
                                {
                                    ("Details", $"Das globale Modul `{name}` wurde erfolgreich entladen und gelöscht")
                                }
                            };
                            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(embedBuilder.Build()));
                        }
                        else
                        {
                            ModernEmbedBuilder embedBuilder = new()
                            {
                                Title = "Fehler",
                                Color = 0xED4245,
                                Fields =
                                {
                                    ("Details", $"Das globale Modul `{name}` konnte nicht entladen werden")
                                }
                            };
                            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = true }.AddEmbed(embedBuilder.Build()));
                        }
                    }
                    else
                    {
                        ModernEmbedBuilder embedBuilder = new()
                        {
                            Title = "Fehler",
                            Color = 0xED4245,
                            Fields =
                            {
                                ("Details", $"Nur die Entwickler des Bots (Maxi#2608) können globale Module verwalten")
                            }
                        };
                        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = true }.AddEmbed(embedBuilder.Build()));
                    }
                }
                else
                {
                    #region Temporary return
                    ModernEmbedBuilder tempEmbedBuilder = new()
                    {
                        Title = "Fehler",
                        Color = 0xED4245,
                        Fields =
                        {
                            ("Details", "Momentan werden nur globale Module unterstützt")
                        }
                    };
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = true }.AddEmbed(tempEmbedBuilder.Build()));
                    return;
                    #endregion
                    if (PermissionManager.HasPermission(ctx.Member, "ManageModules"))
                    {
#warning Unload guild before delete
                        if (false)
                        {
                            File.Delete(guildModule.GetAssemblyPath());
                            ModernEmbedBuilder embedBuilder = new()
                            {
                                Title = "Modul gelöscht",
                                Color = 0x57F287,
                                Fields =
                                {
                                    ("Details", $"Das lokale Modul `{name}` wurde erfolgreich entladen und gelöscht")
                                }
                            };
                            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(embedBuilder.Build()));
                        }
                        else
                        {
                            ModernEmbedBuilder embedBuilder = new()
                            {
                                Title = "Fehler",
                                Color = 0xED4245,
                                Fields =
                                {
                                    ("Details", $"Das lokale Modul `{name}` konnte nicht entladen werden")
                                }
                            };
                            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = true }.AddEmbed(embedBuilder.Build()));
                        }
                    }
                    else
                    {
                        ModernEmbedBuilder embedBuilder = new()
                        {
                            Title = "Fehler",
                            Color = 0xED4245,
                            Fields =
                            {
                                ("Details", "Um diesen Command zu verwenden ist die Iris-Berechtigung `ManageModules` benötigt")
                            }
                        };
                        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = true }.AddEmbed(embedBuilder.Build()));
                    }
                }
            }
        }

        [SlashCommandGroup("permissions", "Verwalten von Berechtigungen")]
        public class PermissionsCommandGroup
        {
            [SlashCustomRequireGuild]
            [SlashRequireIrisPermission("ManagePermissions")]
            [SlashCommand("set", "Setzt eine Berechtigung für eine bestimmte Rolle", false)]
            public async Task PermissionSetCommand(InteractionContext ctx, [Option("role", "Die Rolle, für die die Berechtigung gesetzt werden soll")] DiscordRole role, [Autocomplete(typeof(PermissionAutocompleteProvider))][Option("perm", "Die zu setzende Berechtigung", true)] string perm, [Option("value", "Wert, auf den die Berechtigung gesetzt werden soll")] bool value)
            {
                PermissionManager.SetPermission(ctx.Guild, role, perm, value);

                ModernEmbedBuilder embedBuilder = new()
                {
                    Title = "Berechtigung gesetzt",
                    Color = 0x57F287,
                    Fields =
                    {
                        ("Details", $"Berechtigung `{perm}` für Rolle `{role.Name}` auf `{value}` gesetzt")
                    }
                };

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(embedBuilder.Build()));
            }
            [SlashCustomRequireGuild]
            [SlashRequireIrisPermission("ManagePermissions")]
            [SlashCommand("get", "Liest Berechtigungen")]
            public async Task PermissionGetCommand(InteractionContext ctx, [Option("role", "Die Rolle, für die die Berechtigung gelesen werden soll")] DiscordRole role = null)
            {
                ModernEmbedBuilder embedBuilder;
                if (role != null)
                {
                    // All for one role
                    string[] perms = PermissionManager.GetPermissions(ctx.Guild, role);
                    if (perms.Length > 0)
                    {
                        string permList = "";
                        perms.ForEach(p => permList += "- " + p + '\n');
                        permList = permList.Trim();
                        embedBuilder = new ModernEmbedBuilder
                        {
                            Title = "Berechtigungen gelesen",
                            Color = 0xFEE75C,
                            Fields =
                            {
                                ("Details", permList.Length > 0 ? $"Die Rolle `{role.Name}` hat diese Berechtigungen:\n" + permList : "Für diese Rolle sind keine Berechtigungen vergeben")
                            }
                        };
                    }
                    else
                    {
                        embedBuilder = new ModernEmbedBuilder
                        {
                            Title = "Berechtigungen gelesen",
                            Color = 0xFEE75C,
                            Fields =
                            {
                                ("Details", $"Die Rolle `{role.Name}` hat keine Berechtigungen")
                            }
                        };
                    }
                }
                else
                {
                    // Show everything
                    string permList = "";
                    foreach (DiscordRole item in ctx.Guild.Roles.Values)
                    {
                        string[] perms = PermissionManager.GetPermissions(ctx.Guild, item);
                        if (perms.Length > 0)
                        {
                            permList += '`' + item.Name + "`\n";
                            perms.ForEach(p => permList += "	- " + p + '\n');
                        }
                    }

                    embedBuilder = new ModernEmbedBuilder
                    {
                        Title = "Berechtigungen gelesen",
                        Color = 0xFEE75C,
                        Fields =
                        {
                            ("Details", permList.Length > 0 ? permList : "Auf diesem Server wurden keine Berechtigungen vergeben")
                        }
                    };
                }

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = true }.AddEmbed(embedBuilder.Build()));
            }
            [SlashCustomRequireGuild]
            [SlashRequireIrisPermission("ManagePermissions")]
            [SlashCommand("reset", "Entfernt alle Berechtigungen von der angegebenen Rolle", false)]
            public async Task PermissionResetCommand(InteractionContext ctx, [Option("role", "Die Rolle, von der alle Berechtigungen entfernt werden sollen")] DiscordRole role = null)
            {
                PermissionManager.ResetPermissions(ctx.Guild, role);
                ModernEmbedBuilder embedBuilder = new()
                {
                    Title = "Berechtigungen zurückgesetzt",
                    Color = 0x57F287,
                    Fields =
                    {
                        ("Details", $"Alle vergebenen Berechtigungen für {(role == null ? "diesen Server" : $"die Rolle `{role.Name}`")} wurden zurückgesetzt")
                    }
                };

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(embedBuilder.Build()));
            }
        }
    }
}
