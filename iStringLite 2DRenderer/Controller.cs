using System;
using System.Collections;
using System.Net.Sockets;

namespace iStringLite_2DRenderer
{
    public class Controller
    {
        public static readonly byte[] VALID_COMMANDS = new byte[] {
            0x00, // Command_Update_1bit	
            0x01, // Command_AddressMap	
            0x04, // Command_Update_8bit	
            0x05, // Command_AvailableIDs
            0x06, // Command_Readdress
            0x07, // Command_Update_8bitGreyscale
            0x08, // Command_Update_16bit		
            0x0C, // Command_Update_24bit
            0x10, // Command_SetColour		
            0x13, // Command_Reset			
            0x14, // Command_DynamicReaddress	
            0x15, // Command_SetDeviceType		
            0x16, // Command_EnablePowerLine		
            0x20, // Command_Configure		
            0x21, // Command_SetOutputMode		
            0x22, // Command_SetPWMOutputLevel	
            0x23, // Command_DynamicReaddressType
            0x24, // Command_WriteEffectBlock	
            0x25, // Command_KeepAlive	
            0x26  // Command_EffectTrigger
        };

        public int Id { get; }
        private readonly string name;
        private readonly string hostName;
        private readonly int port;

        public byte[] DataBuffer { get; set; }
        public byte[] PacketBuffer { get; set; }
        private UdpClient udpClient;
        public ArrayList LightPoints { get; }


        public Controller(int id, string name, string hostName, int port, UdpClient udpClient)
        {
            this.Id = id;
            this.name = name;
            this.hostName = hostName;
            this.port = port;
            this.udpClient = udpClient;
            this.LightPoints = new ArrayList();
            this.DataBuffer = new byte[240];
        }

        public LightPoint addLightPoint(byte id, double x, double y, double z)
        {
            LightPoint lightPoint = new LightPoint(id, x, y, z);
            LightPoints.Add(lightPoint);
            return lightPoint;
        }
        
        public void sendCommand(int controlElementID, int lightingElementID, byte command, byte[] data)
        {
            // return if controller ID is invalid
            if (controlElementID < 0 || controlElementID > 255)
                return;

            // return if lighting ID is invalid
            if (lightingElementID < 0 || lightingElementID > 255)
                return;

            // return if command is invalid
            if (Array.IndexOf(VALID_COMMANDS, command) == -1)
                return;
            
            PacketBuffer = new byte[data.Length + 6]; // packet buffer at sie of data + 6 header bytes

            // splits length into two bytes
            byte[] length = BitConverter.GetBytes(PacketBuffer.Length);
            byte lengthHigh = length[1];
            byte lengthLow = length[0];

            byte checksum = 0x00; // empty checksum
            
            // set bytes in packet buffer
            PacketBuffer[0] = (byte) controlElementID;
            PacketBuffer[1] = (byte) lightingElementID;
            PacketBuffer[2] = lengthHigh;
            PacketBuffer[3] = lengthLow;
            PacketBuffer[4] = command;
            PacketBuffer[5] = checksum;

            // set packet data field
            for (int x = 0; x < data.Length; x++)
            {
                PacketBuffer[6 + x] = data[x]; // starts after the header bytes (6)
            }


            // calculate checksum
            for (int x = 0; x < PacketBuffer.Length; x++)
            {
                checksum += PacketBuffer[x];
            }

            PacketBuffer[5] = checksum; // set checksum header

            // send packet
            udpClient.Send(PacketBuffer, PacketBuffer.Length, hostName, port);

            //Console.WriteLine("Sending command {4} to {0}:{1} addressing controller {2} light {3}", hostName, port, controlElementID, lightingElementID, command);
        }
    }
}