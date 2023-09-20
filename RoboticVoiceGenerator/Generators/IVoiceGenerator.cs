using System;

using NAudio.Wave;

namespace RoboticVoiceGenerator
{
    public interface IVoiceGenerator
    {
        ISampleProvider GetVoice(String[] syllables);
    }
}