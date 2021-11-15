using DSharpPlus.Entities;
using System.Collections.Generic;
using System.IO;

namespace IrisLoader.Modules;

internal static class ModuleSettings
{
    private static readonly Dictionary<(ulong, string), object> settings = new();

    internal static T GetSettings<T>(DiscordGuild guild, BaseIrisModule module) where T : new()
    {
        if (!settings.ContainsKey((guild.Id, module.Name)))
            UpdateFromFile<T>(guild, module);
        return (T)settings[(guild.Id, module.Name)];
    }
    internal static void SetSettings<T>(DiscordGuild guild, BaseIrisModule module, T settingsObject)
    {
        settings[(guild.Id, module.Name)] = settingsObject;
        ModuleIO.WriteJson(guild, module, "/settings.json", settingsObject);
    }

    internal static void UpdateFromFile<T>(DiscordGuild guild, BaseIrisModule module) where T : new()
    {
        if (!File.Exists(ModuleIO.GetModuleFileDirectory(guild, module.Name).FullName + "/settings.json"))
        {
            ModuleIO.WriteJson(guild, module, "/settings.json", new T());
        }
        settings[(guild.Id, module.Name)] = ModuleIO.ReadJson<T>(guild, module, "/settings.json");
    }
}
