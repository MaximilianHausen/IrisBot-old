using DSharpPlus;
using DSharpPlus.SlashCommands;

namespace IrisLoader.Modules.Global
{
	/// <summary> Only usable in global modules, will be checked before loading </summary>
	public static class GlobalClientWrapper
	{
		/// <summary> No abstraction needed here, global modules can only be uploaded by the developers </summary>
		public static DiscordShardedClient GetClient() => Loader.Client;
		public static void RegisterCommands<T>() where T : ApplicationCommandModule => Loader.SlashExt.RegisterCommands<T>();
	}
}
