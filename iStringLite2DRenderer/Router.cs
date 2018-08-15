using System;
using System.Collections;
using System.Linq;
using System.Net.Sockets;

namespace iStringLite2DRenderer
{
    /// <summary>
    /// A Router class which contains all connection details and references to the Controllers attached.
    /// </summary>
    public class Router
    {
        // a list of valid commands that can be sent to the iStringLite LEDs
        public static readonly byte[] VALID_COMMANDS = new byte[] {
            //0x00, // Command_Update_1bit	
            //0x01, // Command_AddressMap	
            //0x04, // Command_Update_8bit	
            //0x05, // Command_AvailableIDs
            //0x06, // Command_Readdress
            //0x07, // Command_Update_8bitGreyscale
            //0x08, // Command_Update_16bit		
            0x0C, // Command_Update_24bit
            0x10, // Command_SetColour		
            //0x13, // Command_Reset			
            //0x14, // Command_DynamicReaddress	
            //0x15, // Command_SetDeviceType		
            //0x16, // Command_EnablePowerLine		
            //0x20, // Command_Configure		
            //0x21, // Command_SetOutputMode		
            //0x22, // Command_SetPWMOutputLevel	
            //0x23, // Command_DynamicReaddressType
            //0x24, // Command_WriteEffectBlock	
            //0x25, // Command_KeepAlive	
            //0x26  // Command_EffectTrigger
        };
        
        public string Name { get; }                 // name of the Router (e.g. MK_ROUTER_1._firefly._udp.local)
        public string HostName { get; }             // ip of the Router (e.g. 169.254.1.1)
        public int Port { get; }                    // port of the Router (e.g. 0xCAFE / 51966)

        private readonly UdpClient UdpClient;                // udpClient of the Router used for communication
        public ArrayList Controllers { get; set; }  // an ArrayList containing all Controllers attached to a Router


        public Router(string name, string hostName, int port, UdpClient udpClient)
        {
            this.Name = name;
            this.HostName = hostName;
            this.Port = port;
            this.UdpClient = udpClient;
            
            this.Controllers = new ArrayList();
        }

        /// <summary>
        /// Adds a Controller to the Router with an ID and returns it once added.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Controller addController(int id)
        {
            Controller controller = new Controller(id, Name, HostName, Port, UdpClient);
            Controllers.Add(controller);
            return controller;
        }

        /// <summary>
        /// Loops through all Controllers and returns the one with the queried ID. Returns null if ID cannot be found.
        /// </summary>
        /// <param name="id">The ID of the Controller to be found</param>
        /// <returns></returns>
        public Controller getControllerByID(int id)
        {
            foreach (Controller controller in Controllers)
            {
                if (controller.Id == id)
                    return controller;
            }
            return null;
        }
        
        /// <summary>
        /// Formats and structures a valid packet to be sent to a Router to be forwarded to a specific Controller.
        /// A checksum for the packet is also generated for error detection. 
        /// </summary>
        /// <param name="controlElementID">The Controller to send the command to (0-255, 0: Reserved, 1...254: Controller with given ID, 255: Broadcast to ALL Controllers</param>
        /// <param name="lightingElementID">The LED being controlled (0: Reserved. Processed by Controller, not LED, 1...254: LED with given ID only, 255: Broadcast ALL LEDs</param>
        /// <param name="command">The command to be sent in HEX. Currently, only 0x0C (Update 24Bit) is implemented.</param>
        /// <param name="data">Command specific information (see documentation for full details)</param>
        public void sendCommand(int controlElementID, int lightingElementID, byte command, byte[] data)
        {
            // return if controller ID is invalid
            if (controlElementID < 0 || controlElementID > 255 || controlElementID > Controllers.Count)
                return;
            
            // return if lighting ID is invalid
            if (lightingElementID < 0 || lightingElementID > 255)
                return;

            // return if command is invalid
            if (Array.IndexOf(VALID_COMMANDS, command) == -1)
                return;

            Controller c = (Controller) Controllers[controlElementID - 1];
            
            c.PacketBuffer = new byte[data.Length + 6]; // packet buffer at sie of data + 6 header bytes

            // splits length into two bytes
            byte[] length = BitConverter.GetBytes(c.PacketBuffer.Length);
            byte lengthHigh = length[1];
            byte lengthLow = length[0];

            byte checksum = 0x00; // empty checksum
            
            // set bytes in packet buffer
            c.PacketBuffer[0] = (byte) controlElementID;
            c.PacketBuffer[1] = (byte) lightingElementID;
            c.PacketBuffer[2] = lengthHigh;
            c.PacketBuffer[3] = lengthLow;
            c.PacketBuffer[4] = command;
            c.PacketBuffer[5] = checksum;

            // set packet data field
            for (int x = 0; x < data.Length; x++)
            {
                c.PacketBuffer[6 + x] = data[x]; // starts after the header bytes (6)
            }


            // calculate checksum
            for (int x = 0; x < c.PacketBuffer.Length; x++)
            {
                checksum += c.PacketBuffer[x];
            }

            c.PacketBuffer[5] = checksum; // set checksum header

            // send packet
            UdpClient.Send(c.PacketBuffer, c.PacketBuffer.Length, c.HostName, c.Port);

            //Console.WriteLine("Sending command {4} to {0}:{1} addressing controller {2} light {3}", hostName, port, controlElementID, lightingElementID, command);
        }
    }
}