using System;

namespace AstroneurologicNet.DataLayer {
    public class Entity {
        private HearingData _hearingData;
        private SightData   _sightData;
        private SmellData   _smellData;
        private TasteData   _tasteData;
        private TouchData   _touchData;
        
        public Entity(Random seed) {
            _hearingData = new HearingData(seed);
            _sightData   = new SightData(seed);
            _smellData   = new SmellData(/*seed*/);
            _tasteData   = new TasteData(/*seed*/);
            _touchData   = new TouchData(/*seed*/);
        }
    }
}