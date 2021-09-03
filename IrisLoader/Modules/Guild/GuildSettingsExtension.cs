namespace IrisLoader.Modules.Guild
{
	public class GuildSettingsExtension : BaseIrisModuleExtension
	{
		internal GuildSettingsExtension() { }
		public T GetSettings<T>() where T : new() => ModuleSettings.GetSettings<T>((Module as GuildIrisModule).Guild, Module);
		public void SetSettings<T>(T settingsObject) => ModuleSettings.SetSettings((Module as GuildIrisModule).Guild, Module, settingsObject);

		public void UpdateFromFile<T>() where T : new() => ModuleSettings.UpdateFromFile<T>((Module as GuildIrisModule).Guild, Module);
	}
}
