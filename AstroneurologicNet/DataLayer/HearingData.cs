using System;

namespace AstroneurologicNet.DataLayer {
    public class HearingData {
        public Sound SoundData { get; }
        public HearingData(Random seed) {
            var frequency = seed.Next(20, 20000);
            var amplitude = seed.Next(191);
            var timbre    = seed.Next(1 /*TBD*/);
            var duration  = seed.Next(1 /*TBD*/);

            SoundData = CreateSound(frequency, amplitude, timbre, duration);
        }
        
        private static Sound CreateSound(int frequency, int amplitude, int timbre, int duration) {
            return new Sound() {
                Frequency = frequency, 
                Amplitude = amplitude, 
                Timbre = timbre, 
                Duration = duration
            };
        }

        public struct Sound {
            public int Frequency;
            public int Amplitude;
            public int Timbre;
            public int Duration;
        }
    }
}