using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer
{
    class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Write("Input correct parameters!");
                Console.Write("For example: WebServer.exe <address> <port> <folder>");
                Console.ReadKey();
                return 0;
            }

            try
            {
                MyHttpServer server = MyHttpServer.GetInstance();
                server.Initialize(args[0], args[1], args[2]);

                string command = "";
                while (true)
                {
                    command = Console.ReadLine().Trim().ToLower();
                    switch (command)
                    {
                        case "start":
                            if (server.ServerStateInitialized == ServerState.None)
                            {
                                Utils.PrintMessage("WebServer is not initializing... Please input Quit command...", LogMessageType.Error);
                                break;
                            }
                            if (server.ServerStateRunning == ServerState.Stopped || server.ServerStateRunning == ServerState.None)
                                server.Start();
                            else
                                if (server.ServerStateRunning == ServerState.Started)
                                Utils.PrintMessage("WebServer is running... Please input Stop or Quit command...", LogMessageType.Error);
                            break;
                        case "stop":
                            if (server.ServerStateRunning == ServerState.Started)
                                server.Stop();
                            else
                                Utils.PrintMessage("WebServer is not running... Please input Start command for starting...", LogMessageType.Error);
                            break;
                        case "quit":
                            if (server.ServerStateRunning == ServerState.Started)
                                server.Stop();
                            Utils.PrintMessage("Application WebServer finished...");
                            return 0;
                        default: break;
                    }
                }
            }
            catch (Exception e)
            {
                Utils.PrintMessage(e.Message, LogMessageType.Error);
            }

            return 0;
        }


    }
}
