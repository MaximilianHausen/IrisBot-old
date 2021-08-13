using System.Collections.Generic;
using System.IO;

namespace IrisLoader.IO
{
	public static class SettingsIO
	{
		private static Dictionary<(ulong, string), object> settings = new Dictionary<(ulong, string), object>();

		public static object GetSettings(ulong guildId, string moduleName)
		{
			return settings[(guildId, moduleName)];
		}
		public static void SetSettings<T>(ulong guildId, string moduleName, T settingsObject)
		{
			settings[(guildId, moduleName)] = settingsObject;
			ModuleIO.WriteJson<T>(guildId, moduleName, "/settings.json", settingsObject);
		}

		public static void UpdateSettingsFromFile<T>(ulong guildId, string moduleName) where T : new()
		{
			if (!File.Exists(ModuleIO.GetModuleFileDirectory(guildId, moduleName).FullName + "/settings.json"))
			{
				ModuleIO.WriteJson<T>(guildId, moduleName, "/settings.json", new T());
			}
			settings[(guildId, moduleName)] = ModuleIO.ReadJson<T>(guildId, moduleName, "/settings.json");
		}
	}
}
