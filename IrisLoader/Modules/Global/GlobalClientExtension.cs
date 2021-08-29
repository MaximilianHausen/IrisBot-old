﻿using DSharpPlus;
using DSharpPlus.SlashCommands;
using IrisLoader.Permissions;

namespace IrisLoader.Modules.Global
{
	// This isn't static for consistency with its guild counterpart
	public class GlobalClientExtension : BaseIrisModuleExtension
	{
		internal GlobalClientExtension() { }

		// No abstraction needed here, global modules can only be uploaded by the developers
		public DiscordShardedClient GetClient() => Loader.Client;
		public void RegisterCommands<T>() where T : ApplicationCommandModule
		{
			Loader.SlashExt.RegisterCommands<T>();
			PermissionManager.RegisterPermissions<T>(null);
		}
	}
}
