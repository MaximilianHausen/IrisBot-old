namespace IrisLoader.Modules;

public abstract class GlobalIrisModule : BaseIrisModule
{
    public GlobalIrisConnection Connection { get; private set; }
    public GlobalIrisModule() => Connection = new GlobalIrisConnection(this);
}
