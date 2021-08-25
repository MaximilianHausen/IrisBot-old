using DSharpPlus;

namespace IrisLoader.Modules.Global
{
	/// <summary> Only usable in global modules, will be checked before loading </summary>
	public static class GlobalClientWrapper
	{
		/// <summary> No abstraction needed here, global modules can only be uploaded by the developers </summary>
		public static DiscordShardedClient GetClient() => Loader.GetClient();
	}
}
