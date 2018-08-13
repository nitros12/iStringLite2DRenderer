using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using iStringLite2DRenderer.Effects;

namespace iStringLite2DRenderer
{
    /// <summary>
    /// TODO: This probably needs to be longer than the others, so fill this in when you have time.
    /// </summary>
    public class Renderer
    {
        public static readonly int FPS = 24;    // Frames per second to be rendered
        
        private Scene Scene;                    // loaded from an XML file using the iStringLite Scene format
        private List<Effect> Effects;           // a list of Effects to be rendered to the LEDs
        private UdpClient UdpClient;            // UDP client for communicating with the Routers
        private VideoBuffer VideoBuffer;        // an array of pixel values to be updated before sending to the LEDs
        private Stopwatch Timer;                // timer used to lock FPS

        public static void Main(string[] args)
        {
            Renderer renderer = new Renderer("Scenes/Board.xml");
            renderer.Render();

            /*Color color = Color.FromArgb(255, 0, 0);
            Console.WriteLine(color);
            color = ControlPaint.Dark(color, .1f);
            Console.WriteLine(color);
            color = ControlPaint.Light(color, .56f);
            Console.WriteLine(color);*/
        }
        
        public Renderer(string sceneFileLocation)
        {
            this.UdpClient = new UdpClient();
            this.Scene = new Scene(sceneFileLocation, UdpClient);
            this.VideoBuffer = new VideoBuffer(Scene.VideoBufferWidth, Scene.VideoBufferHeight);
            
            this.Timer = new Stopwatch();
            this.Effects = new List<Effect>();

            //TODO: Used for testing. This will load automatically

            //AddEffect(new FillEffect(0, 0, 0));
            //AddEffect(new AnimatedGifEffect("Images/vaporwave.gif", 0, 0, VideoBuffer)); //, true, VideoBuffer));
            //AddEffect(new MovingTextureEffect("Images/water.png", -100, -100, 100, 100)); //, true, VideoBuffer));
            //AddEffect(new RandomiseEffect(10));
            AddEffect(new ScrollingFillEffect(255, 0, 0, 10));
            //AddEffect(new BreathingEffect(0.001, 0.5, 0.001, 10));
            AddEffect(new BrightnessEffect(0.01));
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
            foreach (Router r in Scene.Routers)
            {
                foreach (Controller c in r.Controllers)
                {
                    foreach (LightPoint l in c.LightPoints)
                    {
                        // sets Controller data to VideoBuffer data
                        //l.data = VideoBuffer.Buffer[l.Py, l.Px]; //TODO: l.data would be a pointer to the DataBuffer section
                        
                        // copies bytes from VideoBuffer to PacketBuffer at an offset of the light * 3 (RGB)
                        Array.Copy(BitConverter.GetBytes(VideoBuffer.Buffer[l.Py, l.Px]), 0, c.DataBuffer, bufferOffset, 3);
                        //Console.WriteLine("X: {0}, Y: {1}", l.Px, l.Py);
                        bufferOffset += 3; // increment by 3 (R, G, B)
                    }
                    bufferOffset = 0;
                    r.sendCommand(c.Id, 255, 0x0C, c.DataBuffer); // sends the data buffer with the command 0x0C (24-bit command)
                    Thread.Sleep(1);
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
    }
}
