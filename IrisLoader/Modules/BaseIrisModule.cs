using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IrisLoader.Modules
{
	public abstract class BaseIrisModule
	{
		/// <summary> The name of this module, read from the assembly </summary>
		public string Name
		{
			get { return GetType().Assembly.GetName().Name; }
		}

		private List<BaseIrisModuleExtension> extensions = new List<BaseIrisModuleExtension>();

		/// <summary> Is called after the module was loaded. </summary>
		public abstract Task Load();
		/// <summary> Called after the GuildDownloadCompleted event is called </summary>
		public abstract Task Ready();
		/// <summary> Is called before the module is unloaded. </summary>
		public abstract Task Unload();

		public void AddExtension(BaseIrisModuleExtension extension)
		{
			if (extensions.Any(e => e.GetType() == extension.GetType())) return;
			extension.Module = this;
			extension.Setup(this);
			extensions.Add(extension);
		}
		public T GetExtension<T>() where T : BaseIrisModuleExtension => extensions.FirstOrDefault(e => e.GetType() == typeof(T)) as T;
	}
}
