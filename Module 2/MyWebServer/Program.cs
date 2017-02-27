using System;
using System.Net;

namespace WebServer
{
    class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.Write("Input correct parameters!");
                Console.Write("For example: WebServer.exe <address> <port> <folderBase> <folderRoot>");
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
                string directoryBase =  args[2];
                string directoryRoot = args[3];

                if (host.Trim().Equals("") || directoryBase.Trim().Equals("") || directoryRoot.Trim().Equals(""))
                {
                    Utils.PrintMessage("Invalid input parameters...", LogMessageType.Error);
                    Console.ReadKey();
                    return 0;
                }


                var server = new MyHttpServer(new Uri(host), Utils.GetUriFromPath(directoryBase), Utils.GetUriFromPath(directoryRoot));
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
