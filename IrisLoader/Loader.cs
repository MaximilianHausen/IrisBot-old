using DSharpPlus;
using IrisLoader.Modules;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace IrisLoader
{
	public abstract class Loader
	{
		public Loader(Config config)
		{
			this.config = config;
		}

		public abstract Task MainAsync();

		protected Config config;

		protected Dictionary<string, IrisModuleReference> globalModules = new Dictionary<string, IrisModuleReference>();
		protected Dictionary<ulong, Dictionary<string, IrisModuleReference>> guildModules = new Dictionary<ulong, Dictionary<string, IrisModuleReference>>();

		internal IrisModuleReference GetModule(ulong? guildId, string moduleName, out bool isGlobal)
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
		internal Dictionary<string, IrisModuleReference> GetGlobalModules() => globalModules;
		internal Dictionary<string, IrisModuleReference> GetGuildModules(ulong guildId) => guildModules.ContainsKey(guildId) ? guildModules[guildId] : new Dictionary<string, IrisModuleReference>();

		internal abstract DiscordClient GetClient(ulong? guildId);
#warning Solve naming and make property
		internal abstract ILogger GetLogger();

		internal Task<bool> IsValidModule(string path)
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
		private DirectoryInfo GetGlobalModuleDirectory()
		{
			Directory.CreateDirectory("./Modules/Global");
			return new DirectoryInfo("./Modules/Global");
		}
		internal async Task<bool> LoadGlobalModuleAsync(string name)
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
		internal async Task<(int, int)> LoadAllGlobalModulesAsync()
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

		internal async Task<bool> UnloadGlobalModuleAsync(string name)
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
		internal async Task<(int, int)> UnloadAllGlobalModulesAsync()
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
	}
}
