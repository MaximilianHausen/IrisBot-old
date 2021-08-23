using IrisLoader.Loader;
using System.IO;
using System.Text.Json;

namespace IrisLoader
{
	internal class Program
	{
		internal static BaseLoader ActiveLoader { get; private set; }

		internal static void Main()
		{
			string configString = File.ReadAllText("./config.json");
			Config config = JsonSerializer.Deserialize<Config>(configString);

			if (config?.Token == null || config?.MySqlPassword == null) return;

			if (config.UseShardedLoader)
			{
				ActiveLoader = new ShardedLoader(config);
				ActiveLoader.MainAsync().GetAwaiter().GetResult();
			}
			else
			{
				ActiveLoader = new StandartLoader(config);
				ActiveLoader.MainAsync().GetAwaiter().GetResult();
			}
		}
	}
}
