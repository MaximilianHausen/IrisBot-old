using DSharpPlus.Entities;
using IrisLoader.Modules;
using System.Collections.Generic;
using System.IO;

namespace IrisLoader.IO
{
	/// <summary> Untested and most likely broken </summary>
	public static class SettingsIO
	{
		private static Dictionary<(ulong, string), object> settings = new Dictionary<(ulong, string), object>();

		public static T GetSettings<T>(DiscordGuild guild, string moduleName)
		{
			return (T)settings[(guild.Id, moduleName)];
		}
		public static void SetSettings<T>(DiscordGuild guild, string moduleName, T settingsObject)
		{
			settings[(guild.Id, moduleName)] = settingsObject;
			ModuleIO.WriteJson(guild, moduleName, "/settings.json", settingsObject);
		}

		public static void UpdateAllGuildsFromFile<T>(BaseIrisModule module) where T : new() => Loader.Client.GetGuilds().ForEach(g => UpdateFromFile<T>(g, module));
		public static void UpdateFromFile<T>(DiscordGuild guild, BaseIrisModule module) where T : new()
		{
			if (!File.Exists(ModuleIO.GetModuleFileDirectory(guild, module.Name).FullName + "/settings.json"))
			{
				ModuleIO.WriteJson(guild, module.Name, "/settings.json", new T());
			}
			settings[(guild.Id, module.Name)] = ModuleIO.ReadJson<T>(guild, module.Name, "/settings.json");
		}
	}
}
