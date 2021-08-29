namespace IrisLoader.Modules
{
	public abstract class BaseIrisModuleExtension
	{
		public BaseIrisModule Module { get; internal set; }

		protected internal virtual void Setup(BaseIrisModule module) { }
	}
}
