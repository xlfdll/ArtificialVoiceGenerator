using System;
using System.Collections.Generic;
using System.Resources;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace RoboticVoiceGenerator
{
    public class AFVoiceGenerator : IVoiceGenerator
    {
        public AFVoiceGenerator(ResourceManager resourceManager)
        {
            this.ResourceManager = resourceManager;
        }

        public ResourceManager ResourceManager { get; }

        public Double Pitch { get; set; }
        public Double TrimStart { get; set; }
        public Double TrimEnd { get; set; }

        public ISampleProvider GetVoice(String[] syllables)
        {
            var samples = this.LoadSamples(syllables);
            var trimmedSamples = this.TrimSamples(samples);
            //var pitchedSamples = this.PitchSamples(trimmedSamples);

            return new ConcatenatingSampleProvider(trimmedSamples);
        }

        #region Steps

        private WaveFileReader[] LoadSamples(String[] syllables)
        {
            Dictionary<String, WaveFileReader> waveStreams = new Dictionary<String, WaveFileReader>();
            List<WaveFileReader> samples = new List<WaveFileReader>();

            foreach (String syllable in syllables)
            {
                String s = syllable;

                if (s == "in")
                {
                    s = "_in";
                }

                try
                {
                    var resourceAudioStream = this.ResourceManager.GetStream(s);
                    WaveFileReader waveStream = null;

                    if (resourceAudioStream != null)
                    {
                        if (!waveStreams.TryGetValue(s, out waveStream))
                        {
                            waveStream = new WaveFileReader(resourceAudioStream);

                            waveStreams.Add(s, waveStream);
                        }

                        samples.Add(waveStream);
                    }
                }
                catch
                {
                    throw;
                }
            }

            return samples.ToArray();
        }

        private ISampleProvider[] TrimSamples(WaveFileReader[] samples)
        {
            List<ISampleProvider> resultSampleProviders = new List<ISampleProvider>();

            foreach (WaveFileReader sample in samples)
            {
                try
                {
                    OffsetSampleProvider offsetSampleProvider = new OffsetSampleProvider(sample.ToSampleProvider())
                    {
                        SkipOverSamples = MathHelper.RoundUpMultiple((Int32)(sample.SampleCount * this.TrimStart), sample.WaveFormat.Channels),
                        TakeSamples = MathHelper.RoundUpMultiple((Int32)(sample.SampleCount * (this.TrimEnd / this.Pitch - this.TrimStart)), sample.WaveFormat.Channels)
                    };

                    resultSampleProviders.Add(offsetSampleProvider);
                }
                catch
                {
                    throw;
                }
            }

            return resultSampleProviders.ToArray();
        }

        //private ISampleProvider[] PitchSamples(ISampleProvider[] sampleProviders)
        //{
        //    foreach (ISampleProvider sampleProvider in sampleProviders)
        //    {
        //        try
        //        {

        //        }
        //        catch
        //        {
        //            throw;
        //        }
        //    }
        //}
    }

    #endregion
}