using DSharpPlus.Entities;

namespace IrisLoader.Modules;

public abstract class GlobalIrisModule : BaseIrisModule
{
    public GlobalIrisConnection Connection { get; private set; }
    public GlobalIrisModule() => Connection = new GlobalIrisConnection(this);

    /// <returns> Whether the module is currently active or not </returns>
    public abstract bool IsActive(DiscordGuild guild);
    /// <summary> Activates/Deactivates a module </summary>
    /// <param name="state"> State to set the module to </param>
    public abstract void SetActive(DiscordGuild guild, bool state);
}
