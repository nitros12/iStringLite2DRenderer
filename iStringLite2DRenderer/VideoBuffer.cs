using System;

namespace iStringLite_2DRenderer
{
    /// <summary>
    /// A VideoBuffer class which stores all rendered pixel data.
    /// The VideoBuffer should be at least 2x the width and height of the light points to improve the resolution and correct for manufacture tolerances.
    /// </summary>
    public class VideoBuffer
    {
        public int Width { get; }                      // width of the VideoBuffer (should be at least 2x the width of the Scene)
        public int Height { get; }                     // height of the VideoBuffer (should be at least 2x the height of the Scene)
        public uint[,] Buffer { get; private set; }    // 2D matrix of pixel data at size mentioned above


        public VideoBuffer(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.Buffer = new uint[height, width];
        }

        /// <summary>
        /// Replaces the current buffer with another.
        /// </summary>
        /// <param name="videoBuffer">New buffer</param>
        public void update(uint[,] videoBuffer)
        {
            this.Buffer = videoBuffer;
        }
    }
}