using System;
using System.Threading;

namespace iStringLite_2DRenderer
{
    public class BreathingEffect : BrightnessEffect
    {
        
        private double minBrightness;
        private double maxBrightness;
        private double increments;
        private int incrementDelayMilis;
        
        public BreathingEffect(double minBrightness, double maxBrightness, double increments, int incrementDelayMillis) : base(minBrightness)
        {
            this.minBrightness = minBrightness;
            this.maxBrightness = maxBrightness;
            this.increments = increments;
            this.incrementDelayMilis = incrementDelayMillis;
        }

        public override void update(ref VideoBuffer videoBuffer)
        {
            base.update(ref videoBuffer);
            
            Brightness += increments; // increases/decreases brightness by the incremental amount

            // if the brightness hits the min or max, the increment is negated
            if (Brightness <= minBrightness || Brightness >= maxBrightness)
                increments = -increments;
            
            Thread.Sleep(incrementDelayMilis);
        }
    }
}