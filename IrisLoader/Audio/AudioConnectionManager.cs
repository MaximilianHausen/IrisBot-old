using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.VoiceNext;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IrisLoader.Audio
{
	internal static class AudioConnectionManager
	{
		// userId, DiscordClient
		private static readonly Dictionary<ulong, DiscordClient> voiceClients = new();
		// channelId, Connection
		private static readonly Dictionary<ulong, IrisAudioConnection> connections = new();

		internal static async Task Connect(IEnumerable<string> audioTokens)
		{
			if (voiceClients.Any()) return;

			// Connect Discord
			foreach (string token in audioTokens)
			{
				var newClient = new DiscordClient(new DiscordConfiguration
				{
					Token = token,
					TokenType = TokenType.Bot,
					Intents = DiscordIntents.AllUnprivileged,
					LogTimestampFormat = "d/M/yyyy hh:mm:ss",
					MinimumLogLevel = LogLevel.Error
				});

				newClient.UseVoiceNext(new VoiceNextConfiguration() { AudioFormat = new AudioFormat(voiceApplication: VoiceApplication.LowLatency) });
				await newClient.ConnectAsync();

				voiceClients.Add(newClient.CurrentUser.Id, newClient);
			}

			Loader.Client.VoiceStateUpdated += VoiceStateUpdated;
		}
		internal static async Task Disconnect()
		{
			if (!voiceClients.Any()) return;

			foreach (var conn in connections)
			{
				conn.Value.VoiceNext.Disconnect();
				connections.Remove(conn.Key);
			}

			foreach (var client in voiceClients)
			{
				await client.Value.DisconnectAsync();
			}

			Loader.Client.VoiceStateUpdated -= VoiceStateUpdated;
		}

		internal static IrisAudioConnection GetConnection(DiscordChannel channel) => connections.GetValueOrDefault(channel.Id);

		private static async Task VoiceStateUpdated(DiscordClient textClient, VoiceStateUpdateEventArgs args)
		{
			// Bot left / got kicked
			if (args.After?.Channel == null && args.Before?.Channel != null && voiceClients.ContainsKey(args.User.Id))
			{
				connections[args.Before.Channel.Id].Dispose();
				connections.Remove(args.Before.Channel.Id);
			}
			// Bot got moved
			else if (args.After?.Channel != null && args.Before?.Channel != null && args.Before.Channel != args.After.Channel && voiceClients.ContainsKey(args.User.Id))
			{
				connections[args.Before.Channel.Id].Dispose();
				connections.Remove(args.Before.Channel.Id);
				connections.Add(args.After.Channel.Id, new IrisAudioConnection(voiceClients[args.User.Id].GetVoiceNext().GetConnection(args.Guild)));
			}
			// User joined
			else if (args.After?.Channel != null && args.Before?.Channel == null && !voiceClients.ContainsKey(args.User.Id))
			{
				// Bot already joined?
				if (connections.ContainsKey(args.After.Channel.Id)) return;

				foreach (var client in voiceClients.Values)
				{
					if (client.Guilds.ContainsKey(args.Guild.Id) && client.GetVoiceNext().GetConnection(args.Guild) == null)
					{
						connections.Add(args.After.Channel.Id, new IrisAudioConnection(await args.After.Channel.AsClient(client).Result.ConnectAsync()));
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
				if (args.Before.Channel.Users.Any(u => !voiceClients.ContainsKey(u.Id))) return;

				connections[args.Before.Channel.Id].VoiceNext.Disconnect();
			}
			// User moved
			else if (args.After?.Channel != null && args.Before?.Channel != null && args.Before.Channel != args.After.Channel && !voiceClients.ContainsKey(args.User.Id))
			{
				#region Leave old
				// Bot already left?
				if (!connections.ContainsKey(args.Before.Channel.Id)) goto Join;
				// Still users left?
				if (args.Before.Channel.Users.Any(u => !voiceClients.ContainsKey(u.Id))) goto Join;

				connections[args.Before.Channel.Id].VoiceNext.Disconnect();
			#endregion
			Join:
				#region Join new
				// Bot already joined?
				if (connections.ContainsKey(args.After.Channel.Id)) return;

				foreach (var client in voiceClients.Values)
				{
					if (client.Guilds.ContainsKey(args.Guild.Id) && client.GetVoiceNext().GetConnection(args.Guild) == null)
					{
						connections.Add(args.After.Channel.Id, new IrisAudioConnection(await args.After.Channel.AsClient(client).Result.ConnectAsync()));
						return;
					}
				}
				#endregion
			}
		}
	}
}
