using System;
using System.Timers;

namespace iStringLite_2DRenderer
{
    public class RandomiseEffect : Effect
    {
        private readonly Random Random;

        public RandomiseEffect(int interval) : base(interval)
        {
            this.Random = new Random();
        }


        public override void update(ref VideoBuffer videoBuffer)
        {
            if (!intervalReached()) return; // only update if interval met
            
            // loop through entire video buffer and generte random values
            for (int y = 0; y < videoBuffer.Height; y++)
            {
                for (int x = 0; x < videoBuffer.Width; x++)
                {
                    // generate random RGB values
                    int r = Random.Next(255);
                    int g = Random.Next(255);
                    int b = Random.Next(255);

                    // bit shift the RGB values into one int
                    int rgb = r << 8 | g;
                    rgb = rgb << 8 | b;

                    videoBuffer.Buffer[y, x] = (uint) rgb; // set the VideoBuffer value at x, y to the randomised RGB value
                }
            }

            isComplete = true;
            reset();
        }

        public override void reset()
        {
            Timer.Restart(); // restart the timer
        }

        public override bool isComplete { get; set; }
    }
}