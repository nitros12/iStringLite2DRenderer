using System;
using System.Collections;

namespace iStringLite2DRenderer.Effects
{
    public class WaterEffect : Effect
    {
        private ArrayList _lines;
        private Random _random;

        public WaterEffect(int interval, VideoBuffer videoBuffer)
        {
            this.Interval = interval;
            this._lines = new ArrayList();
            this._random = new Random();
            play();
            
            for (int y = -videoBuffer.Height; y < 0; y += 8)
            {
                for (int x = 0; x < videoBuffer.Width; x += _random.Next(2))
                {
                    _lines.Add(new Line(x, y + _random.Next(8), 3, 1, 0xFFFFFF));
                }
            }
        }

        public override void update(ref VideoBuffer videoBuffer)
        {
            foreach (Line line in _lines)
            {
                for (int y = 0; y < line.Length; y++)
                {
                    if (line.X >= 0 && line.Y >= 0 && line.X < videoBuffer.Width && line.Y + y < videoBuffer.Height)
                        videoBuffer.setPixel(line.X, line.Y + y, line.Colour);
                }
            }

            if (!intervalReached()) return;
            foreach (Line line in _lines)
            {
                line.Y += _random.Next(3);
                
                if (line.Y >= videoBuffer.Height)
                {
                    line.Y = 0;
                }

                line.X += _random.Next(2);
                if (line.X >= videoBuffer.Width)
                {
                    line.X = 0;
                }
            }
            Timer.Restart();
        }

        public override void reset()
        {
            throw new System.NotImplementedException();
        }

        public override bool isComplete { get; set; }
    }
    
    public class Line
    {
        public int X, Y, Length, Width;
        public uint Colour;

        public Line(int x, int y, int length, int width, uint colour)
        {
            this.X = x;
            this.Y = y;
            this.Length = length;
            this.Width = width;
            this.Colour = colour;
        }

        public void SetPosition(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}