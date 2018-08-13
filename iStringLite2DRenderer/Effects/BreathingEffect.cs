using System;
using System.Threading;

namespace iStringLite_2DRenderer
{
    public class BreathingEffect : BrightnessEffect
    {
        
        private double MinBrightness;
        private double MaxBrightness;
        private double Increments;
        
        public BreathingEffect(double minBrightness, double maxBrightness, double increments, int incrementDelayMillis) : base(minBrightness)
        {
            this.MinBrightness = minBrightness;
            this.MaxBrightness = maxBrightness;
            this.Increments = increments;
            this.Interval = incrementDelayMillis;
        }

        public override void update(ref VideoBuffer videoBuffer)
        {
            base.update(ref videoBuffer);
            
            if (!intervalReached()) return;
            
            Brightness += Increments; // increases/decreases brightness by the incremental amount

            // if the brightness hits the min or max, the increment is negated
            if (Brightness <= MinBrightness || Brightness >= MaxBrightness)
                Increments = -Increments;
        }
    }
}