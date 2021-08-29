using IrisLoader.Modules.Guild;

namespace IrisLoader.Modules.Global
{
	public class GuildIOExtension : BaseIrisModuleExtension
	{
		internal GuildIOExtension() { }
		public T ReadJson<T>(string relPath) => ModuleIO.ReadJson<T>((Module as GuildIrisModule).Guild, Module, relPath);
		public void WriteJson<T>(string relPath, T mapObject) => ModuleIO.WriteJson((Module as GuildIrisModule).Guild, Module, relPath, mapObject);
	}
}
