using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace IrisLoader.Modules
{
	public struct IrisModuleReference
	{
		public IrisModule module;
		public Assembly assembly;
		public AssemblyLoadContext context;
		public FileInfo file;

		public IrisModuleReference(IrisModule module, Assembly assembly, AssemblyLoadContext context, FileInfo file)
		{
			this.module = module;
			this.assembly = assembly;
			this.context = context;
			this.file = file;
		}
	}
}
