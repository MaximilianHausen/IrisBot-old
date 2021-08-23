namespace IrisLoader.Commands
{
	internal interface IRequireIrisPermissionAttribute
	{
		public abstract string Permission { get; }
		public abstract bool RequireGuild { get; }
	}
}
