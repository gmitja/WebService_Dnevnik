using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;

namespace Dnevnik
{
    class Program
    {
        static void Main(string[] args)
        {
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                IPHostEntry IPhost;
                IPhost = Dns.GetHostEntry(Dns.GetHostName());
                string ukaz;
                using (ServiceHost host = new ServiceHost(typeof(ServiceReference1.Service1Client)))
                {
                    BasicHttpBinding binding = new BasicHttpBinding();
                    binding.MaxReceivedMessageSize = int.MaxValue;
                    Uri baseAddress = new Uri("http://localhost:80/WebserviceDnevnik");
                    host.AddServiceEndpoint(typeof(ServiceReference1.IService1), binding, baseAddress);
                    host.Open();

                    Console.WriteLine("Service up and running at:");
                    foreach (IPAddress ip in IPhost.AddressList)
                    {
                        if (ip.AddressFamily == AddressFamily.InterNetwork)
                            Console.WriteLine("Local IP: " + ip);
                    }
                    do
                    {
                        ukaz = Console.ReadLine().ToLower();
                        if (ukaz == "info")
                        {
                            foreach (var ea in host.Description.Endpoints)
                            {
                                Console.WriteLine(ea.Address);
                            }
                            foreach (IPAddress ip in IPhost.AddressList)
                            {
                                if (ip.AddressFamily == AddressFamily.InterNetwork)
                                    Console.WriteLine("Local IP: " + ip);
                            }
                        }

                        else if (ukaz != "exit")
                            Console.WriteLine("Napaka pri ukazu!");
                    } while (ukaz != "exit");
                    host.Close();

                }
            }
            else
            {
                Console.WriteLine("Ni omrezne povezave, priklopite se na omrezje in poskusite ponovno");
                Console.ReadKey();
            }
        }
    }
}
