using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Net.Sockets;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

namespace iStringLite_2DRenderer
{
    public class Renderer
    {
        private Scene scene;
        private List<Effect> effects;
        private UdpClient udpClient;
        private VideoBuffer VideoBuffer;

        public static void Main(string[] args)
        {
            Renderer renderer = new Renderer("Scenes/InfoLabSection.xml", 20, 80);
            renderer.Render();
        }
        
        public Renderer(string sceneFileLocation, int width, int height)
        {
            this.udpClient = new UdpClient(0xCAFE); //TODO: Tidy this UdpClient being passed around, also, dont use hardcoded port
            this.scene = new Scene(sceneFileLocation, udpClient, width, height);
            this.VideoBuffer = new VideoBuffer(scene, width, height);
            this.effects = new List<Effect>();

            
            //TODO: Used for testing. This will load automatically
            AddEffect(new BitmapEffect("Images/heart.bmp"));
            //AddEffect(new BrightnessEffect(0.01));
            AddEffect(new BreathingEffect(0.001, 0.5, 0.001, 10));
            //AddEffect(new RandomiseEffect());
        }
        
        /// <summary>
        /// Adds an Effect to the list of Effects to be rendered in order of effect during the VideoBuffer loop
        /// </summary>
        /// <param name="effect">The Effect to be added to the list</param>
        public void AddEffect(Effect effect)
        {
            effects.Add(effect); // adds the Effect to the Effects list
        }
        
        /// <summary>
        /// Updates the VideoBuffer with the pixel colours to be written to the LEDs by looping through the effects
        /// </summary>
        public void UpdateVideoBuffer()
        {
            foreach (Effect effect in effects)
            {
                effect.update(ref VideoBuffer);
            }
        }

        /// <summary>
        /// Used instead of pointers (this moves the data from the VideoBuffer to the Controller's packet buffer)
        /// </summary>
        public void PopulatePacketBuffers()
        {
            int bufferOffset = 0;
            foreach (Router r in scene.Routers)
            {
                foreach (Controller c in r.Controllers)
                {
                    foreach (LightPoint l in c.LightPoints)
                    {
                        // sets Controller data to VideoBuffer data
                        //l.data = VideoBuffer.Buffer[l.Py, l.Px]; //TODO: Make this into a pointer
                        
                        // copies bytes from VideoBuffer to PacketBuffer at an offset of the light * 3 (RGB)
                        Array.Copy(BitConverter.GetBytes(VideoBuffer.Buffer[l.Py, l.Px]), 0, c.DataBuffer, bufferOffset, 3);
                        bufferOffset += 3; // increment by 3 (R, G, B)
                    }
                    bufferOffset = 0;
                    c.sendCommand(c.Id, 255, 0x0C, c.DataBuffer); // sends the data buffer with the command 0x0C (24-bit command)
                }
            }
        }

        /// <summary>
        /// Loops through all Routers, Controllers and sends the commands necessary
        /// to update the LED colours.
        /// </summary>
        public void UpdateLightPoints()
        {
            // loop through all Routers
            foreach (Router r in scene.Routers)
            {
                // loop through all Controllers on said Router
                foreach (Controller c in r.Controllers)
                {
                    //TODO: potentially split up writing to different routers for traffic shaping
                    c.sendCommand(c.Id, 255, 0x0C, c.PacketBuffer); // sends the data buffer with the command 0x0C (24-bit command)
                    //Thread.Sleep(10); // slight delay to prevent frequent packet sending
                }
            }
        }

        /// <summary>
        /// The main render loop that updates the video buffer (effects) and updates the light points
        /// </summary>
        public void Render()
        {
            while (true)
            {
                UpdateVideoBuffer();
                PopulatePacketBuffers();
                //UpdateLightPoints();
                //Thread.Sleep(1); //TODO: May need to introduce an FPS lock (18 FPS apparently)
            }
        }
    }
}
