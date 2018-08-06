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
        public VideoBuffer VideoBuffer { get; }

        public static void Main(string[] args)
        {
            Renderer renderer = new Renderer("Board.xml", 10, 8);
            //renderer.addEffect(new BitmapEffect(ref videoBuffer));
            renderer.Render();
        }
        
        public Renderer(string sceneFileLocation, int width, int height)
        {
            this.udpClient = new UdpClient(0xCAFE); //TODO: Tidy this UdpClient being passed around
            this.scene = new Scene(sceneFileLocation, udpClient);
            this.VideoBuffer = new VideoBuffer(scene, width, height);
            this.effects = new List<Effect>();

            
            //TODO: Used for testing. This will load automatically
            var videoBuffer = VideoBuffer;
            addEffect(new BitmapEffect(ref videoBuffer));
            addEffect(new BreathingEffect(ref videoBuffer, 0.001, 0.5, 0.001));
        }
        
        /// <summary>
        /// Adds an Effect to the list of Effects to be rendered in order of effect during the VideoBuffer loop
        /// </summary>
        /// <param name="effect">The Effect to be added to the list</param>
        public void addEffect(Effect effect)
        {
            effects.Add(effect); // adds the Effect to the Effects list
        }
        
        /// <summary>
        /// Updates the VideoBuffer with the pixel colours to be written to the LEDs by looping through the effects
        /// </summary>
        public void updateVideoBuffer()
        {
            foreach (Effect effect in effects)
            {
                effect.update();
            }
        }
        
        /// <summary>
        /// Used temporarily instead of pointers (this moves the data from the VideoBuffer to the LightPoint)
        /// </summary>
        public void tempCopyBuffer()
        {
            foreach (Router r in scene.Routers)
                foreach (Controller c in r.Controllers)
                    foreach (LightPoint l in c.LightPoints)
                        // sets light data to the video buffer data
                        l.data = VideoBuffer.Buffer[l.Py, l.Px]; //TODO: Make this into a pointer
        }

        /// <summary>
        /// Loops through all Routers, Controllers, and LightPoints and sends the commands necessary
        /// to update the LED colours.
        /// </summary>
        public void updateLightPoints()
        {
            // loop through all Routers
            foreach (Router r in scene.Routers)
            {
                // loop through all Controllers on said Router
                foreach (Controller c in r.Controllers)
                {
                    byte[] data = new byte[c.LightPoints.Count * 3]; // create a data buffer for the data to be sent as a packet
                    int light = 0;

                    // loop through all LEDs on said Controller
                    foreach (LightPoint l in c.LightPoints)
                    {
                        // gets RGB value from light data
                        byte[] rgb = BitConverter.GetBytes(l.data); //TODO: This is just for testing, shouldn't pass around this much
                        
                        // adds light data to the data buffer to be sent to the Controller
                        data[light] = rgb[2];
                        data[light + 1] = rgb[1];
                        data[light + 2] = rgb[0];
                        light += 3;
                    }

                    light = 0;
                    
                    //TODO: potentially split up writing to different routers for traffic shaping
                    c.sendCommand(c.Id, 255, 0x0C, data); // sends the data buffer with the command 0x0C (24-bit command)
                    Thread.Sleep(1); // slight delay to prevent frequent packet sending
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
                updateVideoBuffer();
                tempCopyBuffer();
                updateLightPoints();
                //Thread.Sleep(1000); //TODO: May need to introduce an FPS lock (18 FPS apparently)
            }
        }
    }
}
