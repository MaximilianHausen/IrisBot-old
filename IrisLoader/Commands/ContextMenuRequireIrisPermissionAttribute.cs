using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using IrisLoader.Permissions;
using System;
using System.Threading.Tasks;

namespace IrisLoader.Commands;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class ContextMenuRequireIrisPermissionAttribute : ContextMenuCheckBaseAttribute, IRequireIrisPermissionAttribute
{
    public string Permission { get; private set; }
    public bool RequireGuild { get; private set; }
    public ContextMenuRequireIrisPermissionAttribute(string permission, bool requireGuild = false)
    {
        Permission = permission;
        RequireGuild = requireGuild;
    }

    public override async Task<bool> ExecuteChecksAsync(ContextMenuContext ctx)
    {
        if (ctx.Guild == null) return !RequireGuild;
        if (PermissionManager.HasPermission(ctx.Member, Permission) || ctx.Member.Permissions.HasPermission(DSharpPlus.Permissions.Administrator))
        {
            return true;
        }
        else
        {
            ModernEmbedBuilder embedBuilder = new()
            {
                Title = "Berechtigung gesetzt",
                Color = 0xED4245,
                Fields =
                {
                    ("Details", $"Um diesen Command zu verwenden ist die Iris-Berechtigung `{Permission}` benötigt")
                }
            };

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = true }.AddEmbed(embedBuilder.Build()));
            return false;
        }
    }
}
