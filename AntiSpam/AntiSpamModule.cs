using System;
using System.Threading.Tasks;
using IrisLoader.Modules;

namespace AntiSpam
{
	public class AntiSpamModule : IrisModule
	{
		public override bool IsActive(ulong guildId)
		{
			throw new NotImplementedException();
		}

		public override Task Load()
		{
			throw new NotImplementedException();
		}

		public override void SetActive(ulong guildId, bool state)
		{
			throw new NotImplementedException();
		}

		public override Task Unload()
		{
			throw new NotImplementedException();
		}
	}
}
