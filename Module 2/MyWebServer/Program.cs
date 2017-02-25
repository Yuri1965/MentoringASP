using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebServer
{
    class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.Write("Input correct parameters!");
                Console.Write("For example: WebServer.exe <address> <port> <folder>");
                Console.ReadKey();
                return 0;
            }

            try
            {
                if (!HttpListener.IsSupported)
                {
                    Utils.PrintMessage("Windows XP SP2 or Server 2003 is required to use the HttpListener class!", LogMessageType.Error);
                    Console.ReadKey();
                    return 0;
                }

                string host = Utils.ParseAddressPort(args[0], args[1]);
                string directory = Utils.ParseDirectoryName(args[2]);

                if (host.Trim().Equals("") || directory.Trim().Equals(""))
                {
                    Utils.PrintMessage("Invalid input parameters...", LogMessageType.Error);
                    Console.ReadKey();
                    return 0;
                }

                MyHttpServer server = new MyHttpServer(new Uri(host), directory);
                server.Start();

            }
            catch (Exception e)
            {
                Utils.PrintMessage(e.Message, LogMessageType.Error);
            }

            return 0;
        }


    }
}
