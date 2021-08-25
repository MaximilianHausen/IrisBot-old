using IrisLoader.Modules;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace IrisLoader
{
	internal struct IrisModuleReference
	{
		internal IrisModule module;
		internal Assembly assembly;
		internal AssemblyLoadContext context;
		internal FileInfo file;

		public IrisModuleReference(IrisModule module, Assembly assembly, AssemblyLoadContext context, FileInfo file)
		{
			this.module = module;
			this.assembly = assembly;
			this.context = context;
			this.file = file;
		}
	}
}
