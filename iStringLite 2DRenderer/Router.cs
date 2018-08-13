using System.Collections;
using System.Net.Sockets;

namespace iStringLite_2DRenderer
{
    public class Router
    {
        public string Name { get; }
        public string HostName { get; }
        public int Port { get; }

        private UdpClient udpClient;
        public ArrayList Controllers { get; set; }


        public Router(string name, string hostName, int port, UdpClient udpClient)
        {
            this.Name = name;
            this.HostName = hostName;
            this.Port = port;
            
            this.Controllers = new ArrayList();
            this.udpClient = udpClient;
        }

        public Controller addController(int id)
        {
            Controller controller = new Controller(id, Name, HostName, Port, udpClient);
            Controllers.Add(controller);
            return controller;
        }

        /***
         * Loops through all Controllers and returns the one with the queried ID.
         */
        public Controller getControllerByID(int id)
        {
            foreach (Controller controller in Controllers)
            {
                if (controller.Id == id)
                    return controller;
            }

            return null;
        }
    }
}