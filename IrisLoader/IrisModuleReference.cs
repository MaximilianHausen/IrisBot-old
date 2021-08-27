using IrisLoader.Modules;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace IrisLoader
{
	internal struct IrisModuleReference<T> where T : BaseIrisModule
	{
		internal T module;
		internal Assembly assembly;
		internal AssemblyLoadContext context;
		internal FileInfo file;

		internal IrisModuleReference(T module, Assembly assembly, AssemblyLoadContext context, FileInfo file)
		{
			this.module = module;
			this.assembly = assembly;
			this.context = context;
			this.file = file;
		}
	}
}
