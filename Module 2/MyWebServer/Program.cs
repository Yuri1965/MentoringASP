using System;
using System.Net;

namespace WebServer
{
    class Program
    {
        public static int Main(string[] args)
        {
            // проверка параметров запуска
            if (args.Length < 3)
            {
                Console.Write("Input correct parameters!\n");
                Console.Write("For example: WebServer.exe <address> <port> <folderBase> <folderRoot>\n");
                Console.Write("Press any key...\n");
                Console.ReadKey();
                return 0;
            }

            try
            {
                // проверим есть ли поддержка HttpListener на рабочей станции
                if (!HttpListener.IsSupported)
                {
                    Utils.PrintMessage("Windows XP SP2 or Server 2003 is required to use the HttpListener class!", LogMessageType.Error);
                    Console.Write("Press any key...\n");
                    Console.ReadKey();
                    return 0;
                }

                // читаем и проверяем параметры запуска
                Uri host = Utils.ParseAddressPort(args[0], args[1]);
                string directoryBase = args[2];
                string directoryRoot = args[3];

                if (host == null || string.IsNullOrEmpty(directoryBase.Trim()) || string.IsNullOrEmpty(directoryRoot.Trim()))
                {
                    Utils.PrintMessage("Invalid input parameters...", LogMessageType.Error);
                    Console.Write("Press any key...\n");
                    Console.ReadKey();
                    return 0;
                }


                var server = new MyHttpServer(host, Utils.GetUriFromPath(directoryBase), Utils.GetUriFromPath(directoryRoot));
                server.Start();
            }
            catch (Exception e)
            {
                Utils.PrintMessage(e.Message, LogMessageType.Error);
                Console.Write("Press any key...\n");
                Console.ReadKey();
            }

            return 0;
        }

    }
}
