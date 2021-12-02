using DSharpPlus.Entities;

namespace IrisLoader.Modules;

public abstract class GuildIrisModule : BaseIrisModule
{
    public GuildIrisConnection Connection { get; private set; }
    public GuildIrisModule() => Connection = new GuildIrisConnection(this);

    public DiscordGuild Guild { get; internal set; }
}
