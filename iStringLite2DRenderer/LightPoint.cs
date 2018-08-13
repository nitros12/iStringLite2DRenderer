using System;

namespace iStringLite2DRenderer
{
    /// <summary>
    /// A LightPoint class which contains LightPoint data such as the LED ID, X, Y and Z positions,
    /// and projected coordinates used for mapping potentially out of line LEDs correctly to a VideoBuffer.
    /// </summary>
    public class LightPoint
    {
        public byte Id { get; }        // 
        
        public double X { get; }       // X coordinate of the LED
        public double Y { get; }       // Y coordinate of the LED
        public double Z { get; }       // Z coordinate of the LED (Currently not implemented, defaults to 0)

        public int Px { get; set; }    // projected X axis to correct manucfacture tolerance and position
        public int Py { get; set; }    // projected Y axis to correct manucfacture tolerance and position
        public int Pz { get; set; }    // projected Z axis to correct manucfacture tolerance and position

        //public uint data; //TODO: a pointer could be used here so data can be set once and already be prepped for sending

        public LightPoint(byte id, double x, double y, double z)
        {
            this.Id = id;
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// Sets the projected X and Y coordinates to one that matches the VideoBuffer based on min and max ranges using the Map function.
        /// </summary>
        /// <param name="minX">The lowest X coordinate of all LEDs</param>
        /// <param name="maxX">The highest X coordinate of all LEDs</param>
        /// <param name="minY">The lowest Y coordinate of all LEDs</param>
        /// <param name="maxY">The highest Y coordinate of all LEDs</param>
        /// <param name="videoBufferWidth">The width of the VideoBuffer used to map the coordinates</param>
        /// <param name="videoBufferHeight">The height of the VideoBuffer used to map the coordinates</param>
        public void CaluclateProjectedCoordinates(double minX, double maxX, double minY, double maxY, int videoBufferWidth, int videoBufferHeight)
        {
            Px = (int) Map(X, minX, maxX, 0, videoBufferWidth - 1);
            Py = (int) Map(Y, minY, maxY, 0, videoBufferHeight - 1);
            Pz = 0; //TODO: Future versions may wish to implement the Z axis
            
            //TODO: This calculation still seems to need some work for flawless scaling
            //Console.WriteLine("Px: {0}, Py: {1}, X: {2}, Y: {3}, {4}", Px, Py, X, Y, Px != X || Py != Y ? "FALSE" : "TRUE");
        }
        
        /// <summary>
        /// Maps a value from one range to another.
        /// </summary>
        /// <param name="value">The value to be mapped</param>
        /// <param name="minA">The minimum value in the original range</param>
        /// <param name="maxA">The maximum value in the original range</param>
        /// <param name="minB">The minimum value in the new range</param>
        /// <param name="maxB">The maximum value in the new range</param>
        /// <returns>The mapped value from one range to another</returns>
        public double Map(double value, double minA, double maxA, double minB, double maxB)
        {
            return minB + (value-minA)*(maxB-minB)/(maxA-minA);
        }
    }
}