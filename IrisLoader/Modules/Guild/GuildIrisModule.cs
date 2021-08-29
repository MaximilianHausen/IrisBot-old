﻿using DSharpPlus.Entities;

namespace IrisLoader.Modules.Guild
{
	public abstract class GuildIrisModule : BaseIrisModule
	{
		public DiscordGuild Guild { get; internal set; }
		/// <returns> Whether the module is currently active or not </returns>
		public abstract bool IsActive();
		/// <summary> Activates/Deactivates a module </summary>
		/// <param name="state"> State to set the module to </param>
		public abstract void SetActive(bool state);
	}
}
