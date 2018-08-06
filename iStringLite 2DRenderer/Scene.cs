using System;
using System.Linq;
using System.Net.Sockets;
using System.Xml.Linq;

namespace iStringLite_2DRenderer
{
    public class Scene
    {
        public string SceneFileLocation { get; set; }
        public XDocument SceneFile { get; set; }
        public Router[] Routers { get; set; }
        public UdpClient UdpClient { get; set; }


        public const string SCENE_ELEMENT = "Scene";
        public const string CONTROLLER_ELEMENT = "FixedIPService";
        public const string LIGHT_ELEMENT = "Light";

        public Scene(string sceneFileLocation, UdpClient udpClient)
        {
            this.SceneFileLocation = sceneFileLocation;
            this.UdpClient = udpClient;
            this.LoadScene();
            this.countComponents();
        }
        
        /***
         * Loads the Scene from the XML file located at the parameter passed into
         * the constructor (sceneFileLocation). An array of Controllers and LightPoints
         * are generated for each valid controller.
         */
        public void LoadScene()
        {
            SceneFile = XDocument.Load(SceneFileLocation); // loads XML document from file location
            XNamespace ns = SceneFile.Root.GetDefaultNamespace(); // get the namespace being used in the XML document
            var routers = SceneFile.Descendants(ns + CONTROLLER_ELEMENT); // get all routers
            Routers = new Router[routers.Count()]; // initialise array of controllers

            int routerCounter = 0;

            // find min and madoubleoordinates
            double minX = routers.Descendants(ns + LIGHT_ELEMENT).Min(e => (double)e.Attribute("x"));
            double maxX = routers.Descendants(ns + LIGHT_ELEMENT).Max(e => (double)e.Attribute("x"));
            double minY = routers.Descendants(ns + LIGHT_ELEMENT).Min(e => (double)e.Attribute("y"));
            double maxY = routers.Descendants(ns + LIGHT_ELEMENT).Max(e => (double)e.Attribute("y"));

            Console.WriteLine("Namespace: {0}", ns);
            Console.WriteLine("Loading {0} Controllers", routers.Count());
            Console.WriteLine("Total number of LightPoints: {0}", routers.Descendants(ns + LIGHT_ELEMENT).Count());
            Console.WriteLine("MinX: {0}, MaxX: {1}, MinY: {2}, MaxY: {3}", minX, maxX, minY, maxY);

            // loop through all routers and load their lights
            foreach (var router in routers)
            {
                int controllerId;
                var lights = router.Descendants(ns + LIGHT_ELEMENT);
                var lightCounter = 0;
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
                    lightPoint.CaluclateProjectedCoordinates(minX, maxX, minY, maxY); // calculate the projected coordinates given the min and max of each
                    lightCounter++;
                }
                
                routerCounter++;
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
        
        /***
         * Combines the bytes for the controller and light IDs and returns
         * and integer of the new value.
         */
        public static int CombineIDBytes(byte controlElementID, byte lightingElementID)
        {
            int combined = controlElementID << 8 | lightingElementID;
            return combined;
        }
        
        /***
         * Separates the concatenated controller and light IDs and returns
         * them as a byte array.
         *
         * Byte[0] = Lighting Element ID
         * Byte[1] = Control Element ID
         */
        public static byte[] SeparateIDBytes(int id)
        {
            return BitConverter.GetBytes(id);
        }

        public bool update(VideoBuffer videoBuffer)
        {
            return true;
            /*foreach (Router r in Routers)
            {
                //Console.WriteLine("Processing router {0}:{1}", r.HostName, r.Port);
                foreach (Controller c in r.Controllers)
                {
                    //Console.WriteLine("Processing controller {0}", c.Id);
                    foreach (LightPoint l in c.LightPoints)
                    {
                        byte[] data = {(byte) R, (byte) G, (byte) B};
                        c.sendCommand(c.Id, l.Id, 0x10, data);
                        Thread.Sleep(1);
                        //Console.WriteLine("Sending command to light {0}:{1}", c.Id, l.Id);
                    }
                }
            }*/
        }
    }
}