using System;
using System.Collections.Generic;
using System.Resources;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace AFVoiceGenerator.Helpers
{
    public class PronounceAudioHelper
    {
        private static Int32 RoundUp(Int32 numToRound, Int32 multiple)
        {
            if (multiple == 0)
            {
                return numToRound;
            }

            Int32 remainder = numToRound % multiple;

            if (remainder == 0)
            {
                return numToRound;
            }

            return numToRound + multiple - remainder;
        }

        public static ISampleProvider[] GetProcessedObjects
            (String[] syllables,
            ResourceManager resourceManager,
            Double pitch,
            Double trimStart,
            Double trimEnd)
        {
            Dictionary<String, WaveFileReader> waveStreams = new Dictionary<String, WaveFileReader>();
            List<ISampleProvider> clipSampleProviders = new List<ISampleProvider>();
            List<ISampleProvider> resultSampleProviders = new List<ISampleProvider>();

            foreach (String syllable in syllables)
            {
                String s = syllable;

                if (s == "in")
                {
                    s = "_in";
                }

                try
                {
                    var resourceAudioStream = resourceManager.GetStream(s);
                    WaveFileReader waveStream = null;

                    if (resourceAudioStream != null)
                    {
                        if (!waveStreams.TryGetValue(s, out waveStream))
                        {
                            waveStream = new WaveFileReader(resourceAudioStream);

                            waveStreams.Add(s, waveStream);
                        }

                        resultSampleProviders.Add(waveStream.ToSampleProvider());

                        OffsetSampleProvider offsetSampleProvider = new OffsetSampleProvider(waveStream.ToSampleProvider())
                        {
                            SkipOverSamples = RoundUp((Int32)(waveStream.SampleCount * trimStart), waveStream.WaveFormat.Channels),
                            TakeSamples = RoundUp((Int32)(waveStream.SampleCount * (trimEnd / pitch - trimStart)), waveStream.WaveFormat.Channels)
                        };

                        clipSampleProviders.Add(offsetSampleProvider);
                    }
                }
                catch
                {
                    throw;
                }
            }

            ISampleProvider concatenatingSampleProvider = new ConcatenatingSampleProvider(clipSampleProviders.ToArray());

            resultSampleProviders.Add(concatenatingSampleProvider);

            return resultSampleProviders.ToArray();
        }

        public static WaveOut WaveOut { get; } = new WaveOut();
    }
}