using System.IO;
using System.Text.Json;

namespace IrisLoader.IO
{
	public static class ModuleIO
	{
		/// <param name="relPath"> Has to begin with one slash </param>
		public static T ReadJson<T>(ulong guildId, string moduleName, string relPath) where T : new()
		{
			string filePath = GetModuleFileDirectory(guildId, moduleName).FullName + relPath;
			if (!relPath.EndsWith(".json") || !File.Exists(filePath))
			{
				return new T();
			}

			string jsonString = File.ReadAllText(filePath);
			T result = JsonSerializer.Deserialize<T>(jsonString);

			return result;
		}
		/// <param name="relPath"> Has to begin with one slash </param>
		public static void WriteJson<T>(ulong guildId, string moduleName, string relPath, T mapObject)
		{
			string filePath = GetModuleFileDirectory(guildId, moduleName).FullName + relPath;
			Directory.CreateDirectory(new FileInfo(filePath).DirectoryName);
			string jsonString = JsonSerializer.Serialize(mapObject);
			File.WriteAllText(filePath, jsonString);
		}

		public static DirectoryInfo GetModuleFileDirectory(ulong guildId, string moduleName)
		{
			DirectoryInfo temp = new DirectoryInfo(GetGuildFileDirectory(guildId).FullName + '/' + moduleName);
			Directory.CreateDirectory(temp.FullName);
			return temp;
		}
		public static DirectoryInfo GetGuildFileDirectory(ulong guildId)
		{
			string moduleFilePath = "./ModuleFiles/" + Program.ActiveLoader.GetClient(guildId).Guilds[guildId].Name + '~' + guildId;
			Directory.CreateDirectory(moduleFilePath);

			return new DirectoryInfo(moduleFilePath);
		}
	}
}
