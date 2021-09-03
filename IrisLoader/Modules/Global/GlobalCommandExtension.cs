using DSharpPlus.SlashCommands;
using IrisLoader.Permissions;

namespace IrisLoader.Modules.Global
{
	public class GlobalCommandExtension : BaseIrisModuleExtension
	{
		internal GlobalCommandExtension() { }
		public void RegisterCommands<T>() where T : ApplicationCommandModule
		{
			Loader.SlashExt.RegisterCommands<T>();
			PermissionManager.RegisterPermissions<T>(null);
			if (Loader.IsConnected) Loader.SlashExt[0].RefreshCommands();
		}
	}
}
