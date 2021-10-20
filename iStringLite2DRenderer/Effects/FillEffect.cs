namespace iStringLite2DRenderer.Effects
{
    public class FillEffect : Effect
    {
        private uint colour;

        public FillEffect(byte r, byte g, byte b)
        {
            this.colour = (uint) b << 8 | g;
            this.colour = this.colour << 8 | r;
        }

        public override void update(ref VideoBuffer videoBuffer)
        {
            for(int x = 0; x < videoBuffer.Width; x++)
                for (int y = 0; y < videoBuffer.Height; y++)
                    videoBuffer.setPixel(x, y, colour);
        }

        public override void reset()
        {
            throw new System.NotImplementedException();
        }

        public override bool isComplete { get; set; }
    }
}
