using DSharpPlus.Entities;

namespace IrisLoader.Modules.Global
{
	public class GlobalSettingsExtension : BaseIrisModuleExtension
	{
		internal GlobalSettingsExtension() { }
		public T GetSettings<T>(DiscordGuild guild) where T : new() => ModuleSettings.GetSettings<T>(guild, Module);
		public void SetSettings<T>(DiscordGuild guild, T settingsObject) => ModuleSettings.SetSettings(guild, Module, settingsObject);

		public void UpdateFromFile<T>() where T : new() => Loader.Client.GetGuilds().ForEach(g => ModuleSettings.UpdateFromFile<T>(g, Module));
	}
}
