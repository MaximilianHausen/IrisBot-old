using DSharpPlus;

namespace IrisLoader.Modules.Global
{
	// This isn't static for consistency with its guild counterpart
	public class GlobalClientExtension : BaseIrisModuleExtension
	{
		internal GlobalClientExtension() { }

		// No abstraction needed here, global modules can only be uploaded by the developers
		public DiscordShardedClient GetClient() => Loader.Client;
	}
}
