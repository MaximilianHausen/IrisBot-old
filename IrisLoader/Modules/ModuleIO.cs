using DSharpPlus.Entities;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IrisLoader.Modules;

internal static class ModuleIO
{
    private static readonly JsonSerializerOptions ignoreNullOptions = new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };

    /// <param name="relPath"> Has to begin with one slash </param>
    internal static T ReadJson<T>(DiscordGuild guild, BaseIrisModule module, string relPath)
    {
        string filePath = GetModuleFileDirectory(guild, module.Name).FullName + relPath;
        if (!relPath.EndsWith(".json") || !File.Exists(filePath))
            return default;

        string jsonString = File.ReadAllText(filePath);
        T result = JsonSerializer.Deserialize<T>(jsonString);

        return result;
    }
    /// <param name="relPath"> Has to begin with one slash </param>
    internal static void WriteJson<T>(DiscordGuild guild, BaseIrisModule module, string relPath, T mapObject)
    {
        string filePath = GetModuleFileDirectory(guild, module.Name).FullName + relPath;
        Directory.CreateDirectory(new FileInfo(filePath).DirectoryName);
        string jsonString = JsonSerializer.Serialize(mapObject, ignoreNullOptions);
        File.WriteAllText(filePath, jsonString);
    }

    internal static DirectoryInfo GetModuleFileDirectory(DiscordGuild guild, string moduleName)
    {
        DirectoryInfo dir = new(GetGuildFileDirectory(guild).FullName + '/' + moduleName);
        Directory.CreateDirectory(dir.FullName);
        return dir;
    }
    internal static DirectoryInfo GetGuildFileDirectory(DiscordGuild guild)
    {
        DirectoryInfo dir = new("./ModuleFiles/" + guild.Id);
        Directory.CreateDirectory(dir.FullName);
        return dir;
    }
}
