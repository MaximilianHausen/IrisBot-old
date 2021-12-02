using System.Threading.Tasks;

namespace IrisLoader.Modules;

public abstract class BaseIrisModule
{
    /// <summary> The name of this module, read from the assembly </summary>
    public string Name => GetType().Assembly.GetName().Name;

    /// <summary> Is called after the module was loaded. </summary>
    public abstract Task Loaded();
    /// <summary> Called after the GuildDownloadCompleted event is called </summary>
    public abstract Task Ready();
    /// <summary> Is called before the module is unloaded. </summary>
    public abstract Task Unloaded();
}
