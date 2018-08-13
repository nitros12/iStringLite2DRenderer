using System;

namespace iStringLite_2DRenderer
{
    public class RandomiseEffect : Effect
    {
        private Random random;

        public RandomiseEffect()
        {
            this.random = new Random();
        }


        public override void update(ref VideoBuffer videoBuffer)
        {
            for (int y = 0; y < videoBuffer.Height; y++)
            {
                for (int x = 0; x < videoBuffer.Width; x++)
                {
                    int r = random.Next(255);
                    int g = random.Next(255);
                    int b = random.Next(255);
                    
                    int rgb = r << 8 | g;
                    rgb = rgb << 8 | b;

                    videoBuffer.Buffer[y, x] = (uint) rgb;
                }
            }
        }

        public override void reset()
        {
            throw new System.NotImplementedException();
        }

        public override bool isComplete { get; }
    }
}