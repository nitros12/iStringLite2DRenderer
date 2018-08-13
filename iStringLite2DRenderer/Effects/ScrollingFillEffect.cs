using System;
using System.Threading;

namespace iStringLite2DRenderer.Effects
{
    public class ScrollingFillEffect : Effect
    {
        private int endX;
        private int endY;
        private uint colour;
        private bool flip;
        private uint[,] grid;

        public ScrollingFillEffect(byte r, byte g, byte b, int incrementDelayMillis) : base(incrementDelayMillis)
        {
            this.endY = 0;
            this.endY = 0;
            this.flip = false;
            
            int rgb = b << 8 | g;
            rgb = rgb << 8 | r;

            this.colour = (uint) rgb;
            this.Interval = incrementDelayMillis;
            grid = new uint[8,10];
        }
        
        public override void update(ref VideoBuffer videoBuffer)
        {
            if (!intervalReached()) return; // only update if interval met
            for (int y = 0; y <= this.endY; y++)
            {
                for (int x = 0; x <= this.endX; x++)
                {
                    videoBuffer.setPixel(flip ? videoBuffer.Width - x - 1 : x , y, colour);
                }
            }

            endX++;

            if (endX >= videoBuffer.Width)
            {
                endX = 0;
                flip = !flip;
                endY++;
            }

            if (endY >= videoBuffer.Height)
            {
                endX = 0;
                endY = 0;
            }
            
            reset();
        }

        public override void reset()
        {
            Timer.Restart(); // restart the timer
        }

        public override bool isComplete { get; set; }
    }
}