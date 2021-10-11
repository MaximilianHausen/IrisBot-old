using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using IrisLoader.Modules;
using IrisLoader.Permissions;
using Microsoft.Extensions.Logging;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;
using System.Threading.Tasks;

namespace IrisLoader
{
	internal static class Loader
	{
		static Loader()
		{
			string configString = File.ReadAllText("./config.json");
			config = JsonSerializer.Deserialize<Config>(configString);
		}

		internal static bool IsConnected { get; private set; }
		internal static DiscordShardedClient Client { get; private set; }
		internal static IReadOnlyDictionary<int, SlashCommandsExtension> SlashExt { get; private set; }

		internal static readonly Config config;
		private static readonly Dictionary<string, IrisModuleReference<GlobalIrisModule>> globalModules = new();
		private static readonly Dictionary<ulong, Dictionary<string, IrisModuleReference<GuildIrisModule>>> guildModules = new();

		internal static void Main() => MainAsync().GetAwaiter().GetResult();

		internal static async Task MainAsync()
		{
			// Create client
			Client = new DiscordShardedClient(new DiscordConfiguration
			{
				Token = config.Token,
				TokenType = TokenType.Bot,
				Intents = DiscordIntents.AllUnprivileged,
				LogTimestampFormat = "d/M/yyyy hh:mm:ss",
				MinimumLogLevel = LogLevel.Information
			});

			await Client.UseInteractivityAsync();

			SlashExt = await Client.UseSlashCommandsAsync();
			SlashExt.RegisterCommands<LoaderCommands>();
			PermissionManager.RegisterPermissions<LoaderCommands>(null);

			Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/ModuleFiles");

			await LoadAllGlobalModulesAsync();

			// Register startup events
			Client.GuildDownloadCompleted += Ready;
			Client.GuildDeleted += GuildDeleted;

			await Audio.AudioConnectionManager.Connect(config.AudioTokens);

			await Client.StartAsync();
			IsConnected = true;

			await Task.Run(() => Console.ReadKey());

			Console.WriteLine();
			Console.WriteLine("Audio: ");
			await Audio.AudioConnectionManager.Disconnect();
			Console.WriteLine("Shards: ");
			await Client.StopAsync();
		}

		private static Task Ready(DiscordClient client, DSharpPlus.EventArgs.GuildDownloadCompletedEventArgs args)
		{
			// List available guilds
			string guildList = "Iris is on the following Servers: ";
			foreach (var guild in client.Guilds.Values) { guildList += guild.Name + '@' + guild.Id + ", "; }
			guildList = guildList.Remove(guildList.Length - 2, 2);
			Logger.Log(LogLevel.Information, 0, "Startup", guildList);

			// Call Module-Ready
			globalModules.ForEach(m => m.Value.module.Ready());
			guildModules.ForEach(g => g.Value.ForEach(m => m.Value.module.Ready()));

			Reminder.LoadRemainingTasks();

			return Task.CompletedTask;
		}

		public static Task GuildDeleted(DiscordClient client, DSharpPlus.EventArgs.GuildDeleteEventArgs args)
		{
			Reminder.AddReminder(TimeSpan.FromDays(7), "Loader", new string[] { args.Guild.Id.ToString() });
			return Task.CompletedTask;
		}
		internal static Task ReminderRecieved(string[] values)
		{
			var parsedId = ulong.Parse(values[0]);
			var guilds = Client.GetGuilds();
			if (!guilds.ContainsKey(parsedId))
				Directory.Delete(ModuleIO.GetGuildFileDirectory(guilds[parsedId]).FullName, true);
			return Task.CompletedTask;
		}

		/// <returns> Array of the names of the invalid Dependencies </returns>
		internal static string[] CheckDependencies(string assemblyPath)
		{
			// Load Assembly
			var scanContext = new AssemblyLoadContext("ModuleScan", true);
			Assembly moduleAssembly;
			using (var fs = new FileStream(assemblyPath, FileMode.Open, FileAccess.Read))
			{
				moduleAssembly = scanContext.LoadFromStream(fs);
			}

			// Scan and unload
			string[] restrictedDependencies = CheckDependencies(moduleAssembly);
			scanContext.Unload();

			return restrictedDependencies;
		}
		/// <returns> Array of the names of the invalid Dependencies </returns>
		internal static string[] CheckDependencies(Assembly assembly)
		{
			IEnumerable<string> referencedAssemblies = assembly.GetReferencedAssemblies().Select(a => a.Name);
			return referencedAssemblies.Where(a => config.AssemblyBlacklist.Contains(a)).ToArray();
		}
		internal static Task<bool> IsValidModule(string path, bool isGlobal)
		{
			// To absolute path
			if (path.StartsWith('.'))
				path = Path.GetFullPath(path);

			// Is dll
			if (!path.EndsWith(".dll") || !File.Exists(path))
			{
				Logger.Log(LogLevel.Debug, 0, "ModuleValidator", "File \"" + path + "\" is not a dll");
				return Task.FromResult(false);
			}
			// Name is okay
			AssemblyName assemblyName = AssemblyName.GetAssemblyName(path);
			if (assemblyName.Name.Length > 32)
			{
				Logger.Log(LogLevel.Debug, 0, "ModuleValidator", "ModuleName \"" + path + "\" cannot be longer than 32 characters");
				return Task.FromResult(false);
			}
			if (assemblyName.Name == "Loader")
			{
				Logger.Log(LogLevel.Debug, 0, "ModuleValidator", "Module \"" + path + "\" cannot named \"Loader\"");
				return Task.FromResult(false);
			}

			// Load Assembly
			var validationContext = new AssemblyLoadContext("ModuleValidation", true);
			Assembly moduleAssembly;
			using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				moduleAssembly = validationContext.LoadFromStream(fs);
			}

			bool isValid = false;

			// Check and unload
			if (isGlobal)
			{
				isValid = moduleAssembly.ExportedTypes.Any(t => typeof(GlobalIrisModule).IsAssignableFrom(t));
			}
			else
			{
				isValid = moduleAssembly.ExportedTypes.Any(t => typeof(GuildIrisModule).IsAssignableFrom(t));
			}
			validationContext.Unload();

			Logger.Log(LogLevel.Debug, 0, "ModuleValidator", "File \"" + path + "\" " + (isValid ? "contains " : "does not contain ") + "a valid module");

			return Task.FromResult(isValid);
		}
		internal static BaseIrisModule GetModuleByType(Type type) => globalModules.Select(m => m.Value.module).FirstOrDefault(m => m.GetType() == type) ?? guildModules.SelectMany(m => m.Value.Values).Select(m => m.module).FirstOrDefault(m => m.GetType() == type) as BaseIrisModule;
		internal static BaseIrisModule GetModuleByName(string name) => globalModules.Select(m => m.Value.module).FirstOrDefault(m => m.Name == name) ?? guildModules.SelectMany(m => m.Value.Values).Select(m => m.module).FirstOrDefault(m => m.Name == name) as BaseIrisModule;

		#region Global Modules
		private static DirectoryInfo GetGlobalModuleDirectory()
		{
			Directory.CreateDirectory("./Modules/Global");
			return new DirectoryInfo("./Modules/Global");
		}
		internal static Dictionary<string, IrisModuleReference<GlobalIrisModule>> GetGlobalModules() => globalModules;
		internal static async Task<bool> LoadGlobalModuleAsync(string name)
		{
			if (name.Length > 32)
			{
				Logger.Log(LogLevel.Warning, 0, "ModuleLoader", $"Global module \"{name}\" was not loaded because name \"{name}\" is longer than 32 characters");
				return false;
			}
			if (globalModules.ContainsKey(name))
			{
				Logger.Log(LogLevel.Warning, 0, "ModuleLoader", $"Global module \"{name}\" was not loaded because it is already loaded");
				return false;
			}

			FileInfo file = GetGlobalModuleDirectory().GetFiles().FirstOrDefault(f => f.Extension == ".dll" && AssemblyName.GetAssemblyName(f.FullName).Name == name);
			if (file == null)
			{
				Logger.Log(LogLevel.Warning, 0, "ModuleLoader", $"Global module \"{name}\" was not loaded because it does not exist");
				return false;
			}
			if (!await IsValidModule(file.FullName, true))
			{
				Logger.Log(LogLevel.Error, 0, "ModuleLoader", $"Global module \"{name}\" was not loaded because it is not a valid module");
				return false;
			}

			// Load Assembly
			var context = new AssemblyLoadContext(name, true);
			Assembly assembly;
			using (var fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
			{
				assembly = context.LoadFromStream(fs);
			}

			// Load module
			Type moduleType = assembly.ExportedTypes.First(t => typeof(GlobalIrisModule).IsAssignableFrom(t));
			GlobalIrisModule module = Activator.CreateInstance(moduleType) as GlobalIrisModule;
			globalModules.Add(name, new IrisModuleReference<GlobalIrisModule>(module, assembly, context, file));
			await module.Load();
			if (IsConnected) _ = module.Ready();

			Logger.Log(LogLevel.Information, 0, "ModuleLoader", "Global module loaded: " + name);
			return true;
		}
		internal static async Task<(int, int)> LoadAllGlobalModulesAsync()
		{
			int totalCount = 0;
			int loadedCount = 0;

			foreach (FileInfo file in GetGlobalModuleDirectory().GetFiles().Where(f => f.Extension == ".dll"))
			{
				totalCount++;
				if (await LoadGlobalModuleAsync(AssemblyName.GetAssemblyName(file.FullName).Name)) loadedCount++;
			}

			Logger.Log(loadedCount == totalCount ? LogLevel.Information : LogLevel.Warning, 0, "ModuleLoader", $"Loaded {loadedCount}/{totalCount} global modules");
			return (loadedCount, totalCount);
		}

		internal static async Task<bool> UnloadGlobalModuleAsync(string name)
		{
			bool isLoaded = globalModules.TryGetValue(name, out IrisModuleReference<GlobalIrisModule> toUnload);
			if (!isLoaded)
			{
				Logger.Log(LogLevel.Warning, 0, "ModuleLoader", $"Global module \"{name}\" could not be unloaded because no module of that name is currently loaded");
				return false;
			}

			await toUnload.module.Unload();
			toUnload.context.Unload();
			globalModules.Remove(name);
			Logger.Log(LogLevel.Information, 0, "ModuleLoader", "Global module unloaded: " + name);
			return true;
		}
		internal static async Task<(int, int)> UnloadAllGlobalModulesAsync()
		{
			int totalCount = 0;
			int unloadedCount = 0;

			foreach (IrisModuleReference<GlobalIrisModule> module in globalModules.Values)
			{
				totalCount++;
				if (await UnloadGlobalModuleAsync(module.module.Name)) unloadedCount++;
			}

			Logger.Log(unloadedCount == totalCount ? LogLevel.Information : LogLevel.Warning, 420, "ModuleLoader", $"Unloaded {unloadedCount}/{totalCount} global modules");
			return (unloadedCount, totalCount);
		}
		#endregion
		#region Guild Modules
		internal static Dictionary<string, IrisModuleReference<GuildIrisModule>> GetGuildModules(DiscordGuild guild) => GetGuildModules(guild.Id);
		internal static Dictionary<string, IrisModuleReference<GuildIrisModule>> GetGuildModules(ulong guildId) => guildModules.ContainsKey(guildId) ? guildModules[guildId] : new Dictionary<string, IrisModuleReference<GuildIrisModule>>();
		#endregion
	}
}
