using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.VoiceNext;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IrisLoader.Audio
{
	public static class AudioConnectionManager
	{
		// userId, DiscordClient
		private static readonly Dictionary<ulong, DiscordClient> voiceClients = new();
		// channelId, Connection
		private static readonly Dictionary<ulong, VoiceNextConnection> connections = new();

		public static async Task Initialize(Config config)
		{
			// Connect Discord
			foreach (string token in config.AudioTokens)
			{
				var newClient = new DiscordClient(new DiscordConfiguration
				{
					Token = token,
					TokenType = TokenType.Bot,
					Intents = DiscordIntents.AllUnprivileged,
					LogTimestampFormat = "d/M/yyyy hh:mm:ss",
					MinimumLogLevel = LogLevel.Error
				});

				newClient.UseVoiceNext(new VoiceNextConfiguration());
				await newClient.ConnectAsync();

				voiceClients.Add(newClient.CurrentUser.Id, newClient);
			}

			Loader.Client.VoiceStateUpdated += VoiceStateUpdated;
		}

		public static async Task VoiceStateUpdated(DiscordClient textClient, VoiceStateUpdateEventArgs args)
		{
			// Bot left / got kicked
			if (args.After?.Channel == null && args.Before?.Channel != null && voiceClients.ContainsKey(args.User.Id))
			{
				connections.Remove(args.Before.Channel.Id);
			}
			// Bot got moved
			else if (args.After?.Channel != null && args.Before?.Channel != null && args.Before.Channel != args.After.Channel && voiceClients.ContainsKey(args.User.Id))
			{
				connections.Remove(args.Before.Channel.Id);
				connections.Add(args.After.Channel.Id, voiceClients[args.User.Id].GetVoiceNext().GetConnection(args.Guild));
			}
			// User joined
			else if (args.After?.Channel != null && args.Before?.Channel == null && !voiceClients.ContainsKey(args.User.Id))
			{
				// Bot already joined?
				if (connections.ContainsKey(args.After.Channel.Id)) return;

				foreach (var client in voiceClients.Values)
				{
					if (client.GetVoiceNext().GetConnection(args.Guild) == null)
					{
						connections.Add(args.After.Channel.Id, await client.GetVoiceNext().ConnectAsync(await args.After.Channel.AsClient(client)));
						return;
					}
				}
			}
			// User left
			else if (args.After?.Channel == null && args.Before?.Channel != null && !voiceClients.ContainsKey(args.User.Id))
			{
				// Bot already left?
				if (!connections.ContainsKey(args.Before.Channel.Id)) return;
				// Still users left?
				if (args.Before.Channel.Users.Count(u => !voiceClients.ContainsKey(u.Id)) > 1) return;

				connections[args.Before.Channel.Id].Disconnect();
			}
		}
	}
}
