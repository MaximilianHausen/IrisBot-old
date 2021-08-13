namespace IrisLoader.Permissions
{
	public struct IrisPermission
	{
		public ulong? guildId;
		public string name;
		public IrisPermission(string name, ulong? guildId)
		{
			this.name = name;
			this.guildId = guildId;
		}
	}
}
