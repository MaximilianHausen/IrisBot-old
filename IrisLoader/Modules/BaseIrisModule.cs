using System.Threading.Tasks;

namespace IrisLoader.Modules
{
	public abstract class BaseIrisModule
	{
		public string Name
		{
			get
			{
				return GetType().Assembly.GetName().Name;
			}
		}

		/// <summary> Is called after the module was loaded. </summary>
		public abstract Task Load();
		/// <summary> Is called before the module is unloaded. </summary>
		public abstract Task Unload();
	}
}
