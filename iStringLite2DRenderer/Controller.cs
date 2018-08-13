using System.Collections;
using System.Net.Sockets;

namespace iStringLite2DRenderer
{
    /// <summary>
    /// A Controller class which contains connection details, the details of the Controller (i.e. ID), and
    /// buffers for sending data to the Controller.
    /// </summary>
    public class Controller
    {

        public static readonly int DATA_BUFFER_SIZE = 240; // the size of the data buffer (80leds * 3colours = 240bytes)

        public int Id { get; }
        public string Name { get; }
        public string HostName { get; }
        public int Port { get; }
        public UdpClient UdpClient { get; }

        public byte[] DataBuffer { get; set; }        // a buffer used to store the data to be sent in a command
        public byte[] PacketBuffer { get; set; }      // a buffer used to create the packet to be sent to the controller
        
        public ArrayList LightPoints { get; }         // an ArrayList of LightPoints used to store all LEDs connected to a Controller


        public Controller(int id, string name, string hostName, int port, UdpClient udpClient)
        {
            this.Id = id;
            this.Name = name;
            this.HostName = hostName;
            this.Port = port;
            this.UdpClient = udpClient;
            
            this.DataBuffer = new byte[DATA_BUFFER_SIZE];
            
            this.LightPoints = new ArrayList();
        }

        /// <summary>
        /// Adds a LightPoint to the Controller using an ID, and its coordinates, and returns it once added.
        /// </summary>
        /// <param name="id">The ID of the LED</param>
        /// <param name="x">The X position of the LED</param>
        /// <param name="y">The Y position of the LED</param>
        /// <param name="z">The Z position of the LED</param>
        /// <returns>The newly reated LightPoint</returns>
        public LightPoint addLightPoint(byte id, double x, double y, double z)
        {
            LightPoint lightPoint = new LightPoint(id, x, y, z);
            LightPoints.Add(lightPoint);
            return lightPoint;
        }
    }
}