using System;

namespace ArtificialVoiceGenerator
{
    public interface IVoiceGenerator
    {
        T GetVoice<T>(String[] syllables);
    }
}