using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Xml;
using System.Xml.Linq;

namespace iStringLite2DRenderer
{
    public class Scene
    {
        private string SceneFileLocation { get; set; }               // location of the Scene file
        private XDocument SceneFile { get; set; }                    // the read XML document
        public Router[] Routers { get; private set; }                       // an array of all Routers in the Scene
        private UdpClient UdpClient { get; set; }                    // UDP client for communicating with the Routers
                    
        public int VideoBufferWidth { get; private set; }                   // the width of the VideoBuffer to be rendered to (should be at least 2x the width of the Scene's max X)
        public int VideoBufferHeight { get; private set; }                  // the height of the VideoBuffer to be rendered to (should be at least 2x the width of the Scene's max Y)

        private double MinX { get; set; }                            // min X of scene
        private double MaxX { get; set; }                            // max X of scene
        private double MinY { get; set; }                            // min Y of scene
        private double MaxY { get; set; }                            // max Y of scene

        public int MaxControllerCount { get; set; }

        private const int VIDEO_BUFFER_RESOLUTION = 1;               // the multiplier of the video buffer resolution
        public const string SCENE_ELEMENT = "Scene";                // XML tag for a Scene
        private const string CONTROLLER_ELEMENT = "FixedIPService";  // XML tag for a Router
        private const string LIGHT_ELEMENT = "Light";                // XML tag for a LightPoint

        public Scene(string sceneFileLocation, UdpClient udpClient)
        {
            this.SceneFileLocation = sceneFileLocation;
            this.UdpClient = udpClient;
            this.Routers = new Router[0];
            
            this.LoadScene(SceneFileLocation);
            this.countComponents();
        }
        
        /// <summary>
        /// Loads the Scene from the XML file located at the parameter passed into
        /// the constructor (sceneFileLocation). An array of Controllers and LightPoints
        /// are generated for each valid controller.
        /// </summary>
        /// <param name="sceneFileLocation">The location of the Scene file to be loaded</param>
        public void LoadScene(string sceneFileLocation)
        {
            try
            {
                SceneFile = XDocument.Load(sceneFileLocation); // loads XML document from file location
                XNamespace ns = SceneFile.Root.GetDefaultNamespace(); // get the namespace being used in the XML document
                var routers = SceneFile.Descendants(ns + CONTROLLER_ELEMENT); // get all routers
                Routers = new Router[routers.Count()]; // initialise array of controllers

                int routerCounter = 0;

                // find min and madoubleoordinates
                MinX = routers.Descendants(ns + LIGHT_ELEMENT).Min(e => (double) e.Attribute("x"));
                MaxX = routers.Descendants(ns + LIGHT_ELEMENT).Max(e => (double) e.Attribute("x"));
                MinY = routers.Descendants(ns + LIGHT_ELEMENT).Min(e => (double) e.Attribute("y"));
                MaxY = routers.Descendants(ns + LIGHT_ELEMENT).Max(e => (double) e.Attribute("y"));

                VideoBufferWidth = (int) MaxX * VIDEO_BUFFER_RESOLUTION + 1; //TODO: Fix the 2x videobuffer resolution
                VideoBufferHeight = (int) MaxY * VIDEO_BUFFER_RESOLUTION + 1;

                Console.WriteLine("VBW: {0}, VBH: {1}, maxX: {2}, maxY: {3}", VideoBufferWidth, VideoBufferHeight, MaxX, MaxY);

                Console.WriteLine("Namespace: {0}", ns);
                Console.WriteLine("Loading {0} Controllers", routers.Count());
                Console.WriteLine("Total number of LightPoints: {0}", routers.Descendants(ns + LIGHT_ELEMENT).Count());
                Console.WriteLine("MinX: {0}, MaxX: {1}, MinY: {2}, MaxY: {3}", MinX, MaxX, MinY, MaxY);
                Console.WriteLine("VidideoBufferWidth: {0}, VideoBufferHeight: {1}", VideoBufferWidth, VideoBufferHeight);

                // loop through all routers and load their lights
                foreach (var router in routers)
                {
                    int controllerId;
                    var lights = router.Descendants(ns + LIGHT_ELEMENT);
                    LightPoint[] lightPoints = new LightPoint[lights.Count()];

                    Console.WriteLine("Router {0} has a total of {1} LightPoints", routerCounter + 1, lights.Count());

                    string name = router.Attribute("name")?.Value;
                    string hostname = router.Attribute("hostname")?.Value;
                    int port = Int32.Parse(router.Attribute("port")?.Value);
                    Routers[routerCounter] = new Router(name, hostname, port, UdpClient);

                    // loop through all lightpoints on specific controller and create an instance
                    foreach (var light in lights)
                    {
                        controllerId = SeparateIDBytes(Int32.Parse(light.Attribute("id")?.Value))[1];

                        byte lightId = SeparateIDBytes(Int32.Parse(light.Attribute("id")?.Value))[0];
                        double x = Double.Parse(light.Attribute("x")?.Value);
                        double y = Double.Parse(light.Attribute("y")?.Value);
                        double z = Double.Parse(light.Attribute("z")?.Value);

                        // find controller in router
                        //TODO: This could be tidied up if the controllers are always in order? Ask Joe
                        Controller controller = Routers[routerCounter].getControllerByID(controllerId); // try to load the controller by its ID

                        // check if controller exists yet
                        if (controller == null)
                            controller = Routers[routerCounter].addController(controllerId); // if not, create a new one with the provided ID

                        // add new LightPopint to controller
                        LightPoint lightPoint = controller.addLightPoint(lightId, x, y, z);
                        lightPoint.CaluclateProjectedCoordinates(MinX, MaxX, MinY, MaxY, this.VideoBufferWidth, this.VideoBufferHeight); // calculate the projected coordinates given the min and max of each
                    }

                    // get max controller count for render loop
                    if (Routers[routerCounter].Controllers.Count > MaxControllerCount)
                        MaxControllerCount = Routers[routerCounter].Controllers.Count;
                    
                    routerCounter++;
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Scene file not found at {0}", sceneFileLocation);
            }
            catch (XmlException e)
            {
                Console.WriteLine("Error reading XML in Scene file at {0}", sceneFileLocation);
            }
            catch (Exception e)
            {
                //TODO: Throw a SceneFileError Exception to end program when an invalid scene was passed
                Console.WriteLine("Error loading Scene file at {0}", sceneFileLocation);
            }
        }

        public void countComponents()
        {
            int routers = 0, controllers = 0, lightpoints = 0;
            
            foreach (Router router in Routers)
            {
                routers++;
                Console.WriteLine("Router {0} has the following: ", routers);
                foreach (Controller controller in router.Controllers)
                {
                    controllers++;
                    
                    foreach (LightPoint lightPoint in controller.LightPoints)
                    {
                        lightpoints++;
                    }
                    
                    Console.WriteLine("\tController {0} on router {1} has {2} lightpoints", controllers, routers, lightpoints);
                    lightpoints = 0;
                }
                
                controllers = 0;
            }
            
            Console.WriteLine("There are a total of {0} routers", routers);
        }
        
        /// <summary>
        /// Combines the bytes for the controller and light IDs and returns and integer of the new value.
        /// </summary>
        /// <param name="controlElementID">Controller ID</param>
        /// <param name="lightingElementID">LED ID</param>
        /// <returns>The two IDs concatenated</returns>
        public static int CombineIDBytes(byte controlElementID, byte lightingElementID)
        {
            int combined = controlElementID << 8 | lightingElementID;
            return combined;
        }
        
        /// <summary>
        /// Separates the concatenated controller and light IDs and returns
        /// them as a byte array.
        /// 
        /// Byte[0] = Lighting Element ID
        /// Byte[1] = Control Element ID
        /// </summary>
        /// <param name="id">The ID of the LightPoint stored in the XML file</param>
        /// <returns>The separated Controller ID and LED ID</returns>
        public static byte[] SeparateIDBytes(int id)
        {
            return BitConverter.GetBytes(id);
        }
    }
}