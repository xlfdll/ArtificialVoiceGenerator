using System;
using System.Collections.Generic;
using System.Resources;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace ArtificialVoiceGenerator
{
    public class AFVoiceGenerator : IVoiceGenerator
    {
        public AFVoiceGenerator(ResourceManager resourceManager)
        {
            this.ResourceManager = resourceManager;
        }

        public ResourceManager ResourceManager { get; }

        public Double TrimStart { get; set; } = 0.1;
        public Double TrimEnd { get; set; } = 0.85;
        public Double Pitch { get; set; } = 1.8;

        public IWaveProvider GetVoice<IWaveProvider>(String[] syllables)
        {
            TimeSpan[] durations = null;

            var samples = this.PrepareSamples(syllables, out durations);
            samples = this.PitchSamples(samples);
            samples = this.TrimSamples(samples, durations);
            var result = new ConcatenatingSampleProvider(samples);

            return (IWaveProvider)(result.ToWaveProvider());
        }

        #region Steps

        private ISampleProvider[] PrepareSamples(String[] syllables, out TimeSpan[] durations)
        {
            
            List<ISampleProvider> samples = new List<ISampleProvider>();
            List<TimeSpan> durationList = new List<TimeSpan>();

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
                    var waveStream = new WaveFileReader(resourceAudioStream);

                    if (resourceAudioStream != null)
                    {
                        samples.Add(waveStream.ToSampleProvider());
                        durationList.Add(waveStream.TotalTime);
                    }
                }
                catch
                {
                    throw;
                }
            }

            durations = durationList.ToArray();

            return samples.ToArray();
        }

        private ISampleProvider[] PitchSamples(ISampleProvider[] samples)
        {
            List<ISampleProvider> results = new List<ISampleProvider>();

            foreach (var sample in samples)
            {
                var pitchSample = new SmbPitchShiftingSampleProvider(sample)
                {
                    PitchFactor = (Single)this.Pitch
                };

                results.Add(pitchSample);
            }

            return results.ToArray();
        }

        private ISampleProvider[] TrimSamples(ISampleProvider[] samples, TimeSpan[] durations)
        {
            List<ISampleProvider> results = new List<ISampleProvider>();

            for (Int32 i = 0; i < samples.Length; i++)
            {
                var trimmedSample = new OffsetSampleProvider(samples[i])
                {
                    SkipOver = TimeSpan.FromMilliseconds(durations[i].TotalMilliseconds * this.TrimStart)
                };

                results.Add(trimmedSample);
            }

            return results.ToArray();
        }

        public static String DecomposePinyin(String text)
        {
            return PinYinHelper.ConvertToPinYin(text);
        }
    }

    #endregion
}