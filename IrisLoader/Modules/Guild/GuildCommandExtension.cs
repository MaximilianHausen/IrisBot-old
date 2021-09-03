using DSharpPlus.SlashCommands;
using IrisLoader.Permissions;

namespace IrisLoader.Modules.Guild
{
	public class GuildCommandExtension : BaseIrisModuleExtension
	{
		internal GuildCommandExtension() { }
		public void RegisterCommands<T>() where T : ApplicationCommandModule
		{
			Loader.SlashExt.RegisterCommands<T>((Module as GuildIrisModule).Guild.Id);
			PermissionManager.RegisterPermissions<T>((Module as GuildIrisModule).Guild);
			if (Loader.IsConnected) Loader.SlashExt[0].RefreshCommands();
		}
	}
}
