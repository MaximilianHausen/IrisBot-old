namespace IrisLoader.Commands
{
	public interface IRequireIrisPermissionAttribute
	{
		public abstract string Permission { get; }
		public abstract bool RequireGuild { get; }
	}
}
