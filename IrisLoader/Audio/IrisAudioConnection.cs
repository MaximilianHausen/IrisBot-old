using DSharpPlus.VoiceNext;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;

namespace IrisLoader.Audio;

public class IrisAudioConnection : IDisposable
{
    internal VoiceNextConnection VoiceNext { get; private set; }

    // Queued tracks
    private readonly Dictionary<DateTime, AudioTrack> singleTracks = new();
    private readonly Queue<AudioTrack> generalQueue = new();

    private Timer packageTimer;

    internal IrisAudioConnection(VoiceNextConnection conn)
    {
        VoiceNext = conn;

        packageTimer = new();
        packageTimer.Interval = 100;
        packageTimer.Elapsed += SendTracks;
        packageTimer.AutoReset = true;
        packageTimer.Enabled = true;
    }
    public void Dispose()
    {
        packageTimer.Elapsed -= SendTracks;
        packageTimer.Dispose();
        packageTimer = null;
    }

    /// <summary> Adds a track to the end of the queue </summary>
    public void QueueTrack(AudioTrack track)
    {
        generalQueue.Enqueue(track);
        track.PlaythoughFinished += NextTrack;
    }
    /// <summary> Queues a track to be played at a specific time </summary>
    public void QueueTrack(AudioTrack track, DateTime time)
    {
        singleTracks.Add(time, track);
        track.PlaythoughFinished += () => singleTracks.Remove(time);
    }

    public void NextTrack() => generalQueue.Dequeue().PlaythoughFinished -= NextTrack;

    internal IEnumerable<AudioTrack> GetPlayingTracks()
    {
        if (generalQueue.Any())
            yield return generalQueue.First();

        foreach (AudioTrack track in singleTracks.Where(t => DateTime.Now > t.Key).Select(t => t.Value))
        {
            yield return track;
        }
    }

    private void SendTracks(object sender, ElapsedEventArgs args)
    {
        AudioTrack[] tracks = GetPlayingTracks().ToArray();

        byte[][] allData = new byte[tracks.Length][];

        for (int i = 0; i < tracks.Length; i++)
        {
            allData[i] = new byte[19200];
            tracks[i].stream.ReadAsync(allData[i], 0, 19200);
        }

        using MemoryStream audioPacket = new(MixAudio(allData));
        audioPacket.CopyToAsync(VoiceNext.GetTransmitSink());

        tracks.ForEach(t => t.UpdateFinishEvent());
    }
    private byte[] MixAudio(byte[][] allData)
    {
        if (allData.Length == 0) return Array.Empty<byte>();
        if (allData.Length == 1) return allData[0];

        int maxLength = 0;
        foreach (byte[] data in allData)
            maxLength = Math.Max(maxLength, data.Length);

        byte[] mixedAudio = new byte[maxLength];

        // Modified from https://www.codeproject.com/articles/501521/how-to-convert-between-most-audio-formats-in-net
        int outputIndex = 0;
        for (int i = 0; i < maxLength; i += 2)
        {
            short[] inputSamples = new short[allData.Length];
            for (int j = 0; j < allData.Length; j++)
            {
                inputSamples[j] = BitConverter.ToInt16(allData[j], i);
            }

            int mixedSample = inputSamples.Sum(s => s / allData.Length);
            byte[] outSample = BitConverter.GetBytes((short)mixedSample);

            mixedAudio[outputIndex++] = outSample[0];
            mixedAudio[outputIndex++] = outSample[1];
        }

        return mixedAudio;
    }
}
