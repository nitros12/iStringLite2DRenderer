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

        public void CaluclateProjectedCoordinates(double minX, double maxX, double minY, double maxY)
        {
            //TODO: Finish this function using LINQ
            Px = (int) X;
            Py = (int) Y;
            
        }
    }
}