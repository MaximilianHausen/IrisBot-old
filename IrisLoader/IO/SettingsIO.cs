using DSharpPlus.Entities;
using System.Collections.Generic;
using System.IO;

namespace IrisLoader.IO
{
	/// <summary> Untested and most likely broken </summary>
	public static class SettingsIO
	{
		private static Dictionary<(ulong, string), object> settings = new Dictionary<(ulong, string), object>();

		public static object GetSettings(DiscordGuild guild, string moduleName)
		{
			return settings[(guild.Id, moduleName)];
		}
		public static void SetSettings<T>(DiscordGuild guild, string moduleName, T settingsObject)
		{
			settings[(guild.Id, moduleName)] = settingsObject;
			ModuleIO.WriteJson(guild, moduleName, "/settings.json", settingsObject);
		}

		public static void UpdateSettingsFromFile<T>(DiscordGuild guild, string moduleName) where T : new()
		{
			if (!File.Exists(ModuleIO.GetModuleFileDirectory(guild, moduleName).FullName + "/settings.json"))
			{
				ModuleIO.WriteJson(guild, moduleName, "/settings.json", new T());
			}
			settings[(guild.Id, moduleName)] = ModuleIO.ReadJson<T>(guild, moduleName, "/settings.json");
		}
	}
}
