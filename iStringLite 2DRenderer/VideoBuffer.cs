using System;

namespace iStringLite_2DRenderer
{
    public class VideoBuffer
    {
        private Scene scene;
        public int Width { get; }
        public int Height { get; }
        public uint[,] Buffer { get; private set; }


        public VideoBuffer(Scene scene, int width, int height)
        {
            this.scene = scene;
            this.Width = width;
            this.Height = height;
            this.Buffer = new uint[height, width];
        }

        public bool update(uint[,] videoBuffer)
        {
            this.Buffer = videoBuffer;
            return true;
        }
    }
}