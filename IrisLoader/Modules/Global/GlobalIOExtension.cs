using DSharpPlus.Entities;

namespace IrisLoader.Modules.Global
{
	public class GlobalIOExtension : BaseIrisModuleExtension
	{
		internal GlobalIOExtension() { }
		public T ReadJson<T>(DiscordGuild guild, string relPath) => ModuleIO.ReadJson<T>(guild, Module, relPath);
		public void WriteJson<T>(DiscordGuild guild, string relPath, T mapObject) => ModuleIO.WriteJson(guild, Module, relPath, mapObject);
	}
}
