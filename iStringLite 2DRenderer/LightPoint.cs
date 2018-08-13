using System;

namespace iStringLite_2DRenderer
{
    public class LightPoint
    {
        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public int Px { get; set; }
        public int Py { get; set; }

        public byte Id { get; }

        public uint data; //TODO: Make this a pointer

        public LightPoint(byte id, double x, double y, double z)
        {
            this.Id = id;
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.data = 0;
        }

        public void CaluclateProjectedCoordinates(double minX, double maxX, double minY, double maxY, int videoBufferWidth, int videoBufferHeight)
        {
            //TODO: Finish this function using LINQ
            Px = (int) map(X, minX, maxX, 0, videoBufferWidth - 1);
            Py = (int) map(Y, minY, maxY, 0, videoBufferHeight - 1);
        }
        
        /// <summary>
        /// Maps a value from one range to another.
        /// </summary>
        /// <param name="value">The value to be mapped</param>
        /// <param name="minA">The minimum value in the original range</param>
        /// <param name="maxA">The maximum value in the original range</param>
        /// <param name="minB">The minimum value in the new range</param>
        /// <param name="maxB">The maximum value in the new range</param>
        /// <returns></returns>
        double map(double value, double minA, double maxA, double minB, double maxB)
        {
            return minB + (value-minA)*(maxB-minB)/(maxA-minA);
        }
    }
}