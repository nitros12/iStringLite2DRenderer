using System;
using System.Threading;

namespace iStringLite_2DRenderer
{
    public class BreathingEffect : BrightnessEffect
    {
        
        private double minBrightness;
        private double maxBrightness;
        private double increments = 0.001;
        
        public BreathingEffect(ref VideoBuffer videoBuffer, double minBrightnes, double maxBrightness, double increments) : base(ref videoBuffer, minBrightnes)
        {
            this.minBrightness = minBrightnes;
            this.maxBrightness = maxBrightness;
            this.increments = increments;
        }

        public override void update()
        {
            base.update();
            
            Brightness += increments; // increases/decreases brightness by the incremental amount

            // if the brightness hits the min or max, the increment is negated
            if (Brightness <= minBrightness || Brightness >= maxBrightness)
                increments = -increments;

            Thread.Sleep(10); // small pause?
        }
    }
}