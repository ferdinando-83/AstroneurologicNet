using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace AstroneurologicNet.DataLayer {
    public class SightData {
        public Color ColorData { get; }

        public SightData(Random seed) {
            var red    = seed.Next(255);
            var green  = seed.Next(255);
            var blue   = seed.Next(255);

            ColorData  = CreateColor(red, green, blue);
        }

        private static Color CreateColor(int red, int green, int blue) {
            return new Color() {
                Red = red, 
                Green = green, 
                Blue = blue
            };
        }
        
        public struct Color {
            public int Red;
            public int Green;
            public int Blue;
        }
    }
}