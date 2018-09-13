using System;

namespace iStringLite2DRenderer.Effects
{
    public class WaterWhitesEffect : Effect
    {
        private uint colour;
        private Random random;

        public WaterWhitesEffect(byte r, byte g, byte b)
        {
            this.colour = (uint) b << 8 | g;
            this.colour = this.colour << 8 | r;
            this.random = new Random();
        }

        public override void update(ref VideoBuffer videoBuffer)
        {
            for (int x = 0; x < videoBuffer.Width; x++)
            {
                for (int y = (videoBuffer.Height / 8) * 7; y < videoBuffer.Height; y++)
                {
                    colour = (uint) random.Next(180, 255) << 8 | (uint) random.Next(180, 255);
                    colour = this.colour << 8 | (uint) random.Next(100, 255);
                    videoBuffer.setPixel(x, y, colour);
                }
            }
        }

        public override void reset()
        {
            throw new System.NotImplementedException();
        }

        public override bool isComplete { get; set; }
    }
}