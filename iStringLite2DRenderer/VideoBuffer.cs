namespace iStringLite2DRenderer
{
    /// <summary>
    /// A VideoBuffer class which stores all rendered pixel data.
    /// The VideoBuffer should be at least 2x the width and height of the light points to improve the resolution and correct for manufacture tolerances.
    /// </summary>
    public class VideoBuffer
    {
        public int Width { get; }                      // width of the VideoBuffer (should be at least 2x the width of the Scene)
        public int Height { get; }                     // height of the VideoBuffer (should be at least 2x the height of the Scene)
        public uint[,] Buffer { get; set; }    // 2D matrix of pixel data at size mentioned above


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

        /// <summary>
        /// Sets a byte in the buffer to a specified colour.
        /// Useful for ease of confusion in X and Y coordinates (Buffer[y, x]).
        /// </summary>
        /// <param name="x">Pixel's X coordinate to be set</param>
        /// <param name="y">Pixel's Y coordinate to be set</param>
        /// <param name="colour">The colour to set the pixel to</param>
        public void setPixel(int x, int y, uint colour)
        {
            Buffer[y, x] = colour;
        }

        /// <summary>
        /// Gets the pixel value stored in the Buffer at a specified X and Y coordinate.
        /// Useful for ease of confusing in X and Y coordinates (Buffer[y, x]).
        /// </summary>
        /// <param name="x">Pixel's X coordinate to be returned</param>
        /// <param name="y">Pixel's Y coordinate to be returned</param>
        /// <returns>The colour of the pixel at the X and Y coordinates</returns>
        public uint getPixel(int x, int y)
        {
            return Buffer[y, x];
        }
    }
}