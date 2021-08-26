using DSharpPlus;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using IrisLoader.Commands;
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

		public static readonly Config config;
		internal static DiscordShardedClient Client { get; private set; }
		internal static IReadOnlyDictionary<int, SlashCommandsExtension> SlashExt { get; private set; }
		private static Dictionary<string, IrisModuleReference> globalModules = new();
		private static Dictionary<ulong, Dictionary<string, IrisModuleReference>> guildModules = new();

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
			await LoadAllGlobalModulesAsync();

			// Register startup events
			Client.GuildDownloadCompleted += Ready;

			await Client.StartAsync();
			Console.ReadLine();
			await Client.StopAsync();
		}

		private static Task Ready(DiscordClient client, DSharpPlus.EventArgs.GuildDownloadCompletedEventArgs args)
		{
			// List available guilds
			string guildList = "Iris is on the following Servers: ";
			foreach (var guild in client.Guilds.Values) { guildList += guild.Name + '@' + guild.Id + ", "; }
			guildList = guildList.Remove(guildList.Length - 2, 2);
			Logger.Log(LogLevel.Information, 0, "Startup", guildList);
			return Task.CompletedTask;
		}

		internal static IrisModuleReference GetModule(ulong? guildId, string moduleName, out bool isGlobal)
		{
			if (guildId == null)
			{
				if (globalModules.ContainsKey(moduleName))
				{
					isGlobal = true;
					return globalModules[moduleName];
				}
			}
			else
			{
				if (globalModules.ContainsKey(moduleName))
				{
					isGlobal = true;
					return globalModules[moduleName];
				}
				else if (guildModules.ContainsKey(guildId.Value) && guildModules[guildId.Value].ContainsKey(moduleName))
				{
					isGlobal = false;
					return guildModules[guildId.Value][moduleName];
				}
			}

			isGlobal = false;
			return default;
		}
		internal static void CheckDependencies(string assemblyPath, out IEnumerable<string> restrictedDependencies)
		{
			// Load Assembly
			AssemblyLoadContext scanContext = new AssemblyLoadContext("ModuleScan", true);
			Assembly moduleAssembly;
			using (FileStream fs = new FileStream(assemblyPath, FileMode.Open, FileAccess.Read))
			{
				moduleAssembly = scanContext.LoadFromStream(fs);
			}

			// Scan and unload
			CheckDependencies(moduleAssembly, out restrictedDependencies);
			scanContext.Unload();
			GC.Collect();
		}
		internal static void CheckDependencies(Assembly assembly, out IEnumerable<string> restrictedDependencies)
		{
			List<string> restrictedList = new List<string>();
			// Scan dependencies
			IEnumerable<string> referencedAssemblies = assembly.GetReferencedAssemblies().Select(a => a.Name);
			restrictedList.AddRange(referencedAssemblies.Where(a => config.AssemblyBlacklist.Contains(a)));

			// Roughly scan namespaces
			string[] blockedNamespaces = new string[] { "IrisLoader.Modules.Global", "IrisLoader.Modules.Guild" };
			List<string> usedNamespaces = new List<string>();
			foreach (Type type in assembly.DefinedTypes)
			{
				usedNamespaces.AddRange(type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Select(f => f.FieldType.Namespace).Distinct());
				usedNamespaces.AddRange(type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Select(p => p.PropertyType.Namespace).Distinct());
				usedNamespaces.AddRange(type.GetEvents(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Select(e => e.EventHandlerType.Namespace).Distinct());
				usedNamespaces.AddRange(type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Select(m => m.ReturnType.Namespace).Distinct());
				foreach (MethodInfo method in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
				{
					usedNamespaces.AddRange(method.GetMethodBody().LocalVariables.Select(v => v.LocalType.Namespace));
				}
				usedNamespaces = usedNamespaces.Distinct().ToList();
			}
			restrictedList.AddRange(usedNamespaces.Where(n => blockedNamespaces.Contains(n)));

			restrictedDependencies = restrictedList;
		}
		internal static Task<bool> IsValidModule(string path)
		{
			// To absolute path
			if (path.StartsWith('.'))
				path = Path.GetFullPath(path);
			// Is dll
			if (!path.EndsWith(".dll") || !File.Exists(path)) return Task.FromResult(false);

			bool isValid = false;

			// Load Assembly
			AssemblyLoadContext validationContext = new AssemblyLoadContext("ModuleValidation", true);
			Assembly moduleAssembly;
			using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				moduleAssembly = validationContext.LoadFromStream(fs);
			}

			// Check and unload
			isValid = moduleAssembly.ExportedTypes.Any(t => typeof(IrisModule).IsAssignableFrom(t));
			validationContext.Unload();

			GC.Collect();
			Logger.Log(LogLevel.Debug, 0, "ModuleValidator", "File \"" + path + "\" " + (isValid ? "contains " : "does not contain ") + "a valid module");

			return Task.FromResult(isValid);
		}

		#region Global Modules
		private static DirectoryInfo GetGlobalModuleDirectory()
		{
			Directory.CreateDirectory("./Modules/Global");
			return new DirectoryInfo("./Modules/Global");
		}
		internal static Dictionary<string, IrisModuleReference> GetGlobalModules() => globalModules;
		internal static async Task<bool> LoadGlobalModuleAsync(string name)
		{
			bool success = false;

			DirectoryInfo moduleDirectory = GetGlobalModuleDirectory();

			if (globalModules.ContainsKey(name))
			{
				Logger.Log(LogLevel.Warning, 0, "ModuleLoader", $"Global module \"{name}\" was not loaded because it is already loaded");
				return false;
			}

			// Find and load module in isolated context
			foreach (FileInfo file in moduleDirectory.GetFiles().Where(f => f.Extension == ".dll"))
			{
				if (AssemblyName.GetAssemblyName(file.FullName).Name != name) continue;
				if (!await IsValidModule(file.FullName)) continue;

				// Load Assembly
				AssemblyLoadContext context = new AssemblyLoadContext(name, true);
				Assembly assembly;
				using (FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
				{
					assembly = context.LoadFromStream(fs);
				}

				// Load module
				Type moduleType = assembly.ExportedTypes.Where(t => typeof(IrisModule).IsAssignableFrom(t)).First();
				IrisModule module = (IrisModule)Activator.CreateInstance(moduleType);
				globalModules.Add(name, new IrisModuleReference(module, assembly, context, file));
				await module.Load();
				success = true;

				break;
			}

			Logger.Log(success ? LogLevel.Information : LogLevel.Error, 0, "ModuleLoader", (success ? "Global module loaded: " : "Global module could not be loaded: ") + name);
			return success;
		}
		internal static async Task<(int, int)> LoadAllGlobalModulesAsync()
		{
			int moduleCount = 0;
			int loadedCount = 0;

			DirectoryInfo moduleDirectory = GetGlobalModuleDirectory();

			// Load Modules
			foreach (FileInfo file in moduleDirectory.GetFiles().Where(f => f.Extension == ".dll"))
			{
				moduleCount++;
				string moduleName = AssemblyName.GetAssemblyName(file.FullName).Name;
				if (await LoadGlobalModuleAsync(moduleName)) loadedCount++;
			}

			Logger.Log(loadedCount == moduleCount ? LogLevel.Information : LogLevel.Warning, 0, "ModuleLoader", $"Loaded {loadedCount}/{moduleCount} global modules");
			return (loadedCount, moduleCount);
		}

		internal static async Task<bool> UnloadGlobalModuleAsync(string name)
		{
			bool isLoaded = globalModules.TryGetValue(name, out IrisModuleReference toUnload);
			if (!isLoaded)
			{
				Logger.Log(LogLevel.Warning, 0, "ModuleLoader", $"Global module \"{name}\" was not unloaded because it is not loaded");
				return false;
			}

			await toUnload.module.Unload();
			toUnload.context.Unload();
			globalModules.Remove(name);
			GC.Collect();
			Logger.Log(LogLevel.Information, 0, "ModuleLoader", "Global module unloaded: " + name);
			return true;
		}
		internal static async Task<(int, int)> UnloadAllGlobalModulesAsync()
		{
			int moduleCount = 0;
			int unloadedCount = 0;

			foreach (IrisModuleReference item in globalModules.Values)
			{
				moduleCount++;
				if (await UnloadGlobalModuleAsync(item.module.Name)) unloadedCount++;
			}

			Logger.Log(unloadedCount == moduleCount ? LogLevel.Information : LogLevel.Warning, 420, "ModuleLoader", $"Unloaded {unloadedCount}/{moduleCount} global modules");
			return (unloadedCount, moduleCount);
		}
		#endregion
		#region Guild Modules
		internal static Dictionary<string, IrisModuleReference> GetGuildModules(ulong guildId) => guildModules.ContainsKey(guildId) ? guildModules[guildId] : new Dictionary<string, IrisModuleReference>();
		#endregion
	}
}
