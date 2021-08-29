using DSharpPlus.Entities;
using System.IO;
using System.Text.Json;

namespace IrisLoader.Modules
{
	internal static class ModuleIO
	{
		/// <param name="relPath"> Has to begin with one slash </param>
		internal static T ReadJson<T>(DiscordGuild guild, BaseIrisModule module, string relPath)
		{
			string filePath = GetModuleFileDirectory(guild, module).FullName + relPath;
			if (!relPath.EndsWith(".json") || !File.Exists(filePath))
				return default;

			string jsonString = File.ReadAllText(filePath);
			T result = JsonSerializer.Deserialize<T>(jsonString);

			return result;
		}
		/// <param name="relPath"> Has to begin with one slash </param>
		internal static void WriteJson<T>(DiscordGuild guild, BaseIrisModule module, string relPath, T mapObject)
		{
			string filePath = GetModuleFileDirectory(guild, module).FullName + relPath;
			Directory.CreateDirectory(new FileInfo(filePath).DirectoryName);
			string jsonString = JsonSerializer.Serialize(mapObject);
			File.WriteAllText(filePath, jsonString);
		}

		internal static DirectoryInfo GetModuleFileDirectory(DiscordGuild guild, BaseIrisModule module)
		{
			DirectoryInfo dir = new DirectoryInfo(GetGuildFileDirectory(guild).FullName + '/' + module.Name);
			Directory.CreateDirectory(dir.FullName);
			return dir;
		}
		internal static DirectoryInfo GetGuildFileDirectory(DiscordGuild guild)
		{
			DirectoryInfo dir = new DirectoryInfo("./ModuleFiles/" + guild.Id);
			Directory.CreateDirectory(dir.FullName);
			return dir;
		}
	}
}
