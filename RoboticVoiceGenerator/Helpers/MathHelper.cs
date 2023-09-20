using System;

namespace RoboticVoiceGenerator
{
    public static class MathHelper
    {
        public static Int32 RoundUpMultiple(Int32 numToRound, Int32 multiple)
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
    }
}