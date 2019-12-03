﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Xml.Linq;
using iStringLite2DRenderer.Effects;

namespace iStringLite2DRenderer
{
    /// <summary>
    /// TODO: This probably needs to be longer than the others, so fill this in when you have time.
    /// </summary>
    public class Renderer
    {
        public static readonly int FPS = 15;    // Frames per second to be rendered
        
        private Scene Scene;                    // loaded from an XML file using the iStringLite Scene format
        private List<Effect> Effects;           // a list of Effects to be rendered to the LEDs
        private UdpClient UdpClient;            // UDP client for communicating with the Routers
        private VideoBuffer VideoBuffer;        // an array of pixel values to be updated before sending to the LEDs
        private Stopwatch Timer;                // timer used to lock FPS
        
        public static void Main(string[] args)
        {
            Renderer renderer = new Renderer("/src/iStringLite2DRenderer/Scenes/InfoLab.xml");
            
            renderer.Render();
        }
        
        public Renderer(string sceneFileLocation)
        {
            this.UdpClient = new UdpClient();
            this.Scene = new Scene(sceneFileLocation, UdpClient);
            this.VideoBuffer = new VideoBuffer(Scene.VideoBufferWidth, Scene.VideoBufferHeight);
            
            this.Timer = new Stopwatch();
            this.Effects = new List<Effect>();

            Utils.SoftLoadEffects(out Effects, VideoBuffer);
        }
        
        /// <summary>
        /// Adds an Effect to the list of Effects to be rendered in order of effect during the VideoBuffer loop
        /// </summary>
        /// <param name="effect">The Effect to be added to the list</param>
        public void AddEffect(Effect effect)
        {
            Effects.Add(effect); // adds the Effect to the Effects list
        }
        
        /// <summary>
        /// Updates the VideoBuffer with the pixel colours to be written to the LEDs by looping through the effects.
        /// It should be noted that Effects are rendered in the order they were added and will overwrite pixel data
        /// from previous Effects.
        /// </summary>
        public void UpdateVideoBuffer()
        {
            foreach (Effect effect in Effects)
            {
                effect.update(ref VideoBuffer);
                //Console.WriteLine("Rendering {0}", effect.GetType().Name);
            }
        }

        /// <summary>
        /// Moves the data from the VideoBuffer to the Controller's packet buffer and sends the command
        /// </summary>
        public void UpdateLightPoints()
        {
            int bufferOffset = 0;
            Router r;
            Controller c;
            
            // loops through the routers and sequentially selects the next one in line (offset, nice for traffic shaping delays)
            for (int cIndex = 0; cIndex < Scene.MaxControllerCount; cIndex++)
            {
                for (int rIndex = 0; rIndex < Scene.Routers.Length; rIndex++)
                {
                    if (Scene.Routers[rIndex].Controllers.Count > cIndex) // if unbalanced controller count per router, continue
                    {
                        r = Scene.Routers[rIndex];
                        c = (Controller) r.Controllers[cIndex];
                        //Console.WriteLine("R: {0}, C: {1}", rIndex, cIndex);

                        foreach (LightPoint l in c.LightPoints)
                        {
                            // sets Controller data to VideoBuffer data
                            //l.data = VideoBuffer.Buffer[l.Py, l.Px]; //TODO: l.data would be a pointer to the DataBuffer section

                            // copies bytes from VideoBuffer to PacketBuffer at an offset of the light * 3 (RGB)
                            Array.Copy(BitConverter.GetBytes(VideoBuffer.Buffer[l.Py, l.Px]), 0, c.DataBuffer, bufferOffset, 3);
                            bufferOffset += 3; // increment by 3 (R, G, B)
                        }

                        bufferOffset = 0;
                        r.sendCommand(c.Id, 255, 0x0C, c.DataBuffer); // sends the data buffer with the command 0x0C (24-bit command)
                    }
                }
            }
        }
        
        /// <summary>
        /// The main render loop that updates the video buffer (Effects) and updates all LightPoints with
        /// their new values.
        /// </summary>
        public void Render()
        {
            Timer.Start();
            int frame = 1;
            while (true)
            {
                //TODO: 
                if (Timer.ElapsedMilliseconds <= 1000 / FPS) continue; // skip while loop if not ready to update
                UpdateVideoBuffer();
                UpdateLightPoints();
                Timer.Restart();
                
                //Console.WriteLine("FRAME {0}/{1}", frame, FPS);
                
                // frame counter for dubugging
                if (frame == FPS)
                    frame = 1;
                else
                    frame++;
            }
        }
        
        /// <summary>
        /// Clears the display before exiting the application.
        /// TODO: This currently doesn't work as the application is a console application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnProcessExit (object sender, EventArgs e)
        {
            Console.WriteLine ("Clearing display...");
            Array.Clear(VideoBuffer.Buffer, 0, VideoBuffer.Buffer.Length); // zero out VideoBuffer
            UpdateLightPoints(); // update the light points with zerod buffer
        }
    }
}
