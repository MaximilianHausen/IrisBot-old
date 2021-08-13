using System.Threading.Tasks;

namespace IrisLoader.Modules
{
	public abstract class IrisModule
	{
		public string Name { get; }

		/// <summary> Is called after the module was loaded. </summary>
		public abstract Task Load();
		/// <summary> Is called before the module is unloaded. </summary>
		public abstract Task Unload();

		/// <returns> All permissions required for this module </returns>
		public abstract string[] GetPermissions();

		/// <param name="guildId"> Can be ignored on guildmodules </param>
		/// <returns> Whether the module is currently active or not </returns>
		public abstract bool IsActive(ulong guildId);
		/// <summary> Activates/Deactivates a module </summary>
		/// <param name="guildId"> Can be ignored on guildmodules </param>
		/// <param name="state"> State to set the module to </param>
		public abstract void SetActive(ulong guildId, bool state);
	}
}
