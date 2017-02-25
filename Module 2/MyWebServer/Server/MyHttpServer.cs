using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebServer
{
    public class MyHttpServer
    {
        private string rootDirectory;
        private Uri host;

        private ServerState serverStateRunning = ServerState.None;
        private ServerState serverStateInitialized = ServerState.None;
        public ServerState ServerStateRunning { get { return serverStateRunning; } }
        public ServerState ServerStateInitialized { get { return serverStateInitialized; } }

        private HttpListener serverListener;
        private Thread serverThread;
        
        private static readonly MyHttpServer instance = new MyHttpServer();
 
        private MyHttpServer()
        {
        }

        public static MyHttpServer GetInstance()
        {
            return instance;
        }

        public void Start()
        {
            if (serverStateRunning == ServerState.Started)
            {
                Utils.PrintMessage("WebServer is running! Please stop WebServer...", LogMessageType.Error);
                return;
            }

            if (serverStateInitialized == ServerState.None)
            {
                Utils.PrintMessage("WebServer is not initializing! Please initializing WebServer with parameters...", LogMessageType.Error);
                return;
            }

            serverListener.Start();

            serverStateRunning = ServerState.Started;
            Utils.PrintMessage("WebServer started...", LogMessageType.Error);
            
            ListenProcess();
        }

        private async void ListenProcess()
        {
            while (true)
            {
                try
                {
                    if (serverStateRunning == ServerState.Started)
                    {
                        HttpListenerContext context = await serverListener.GetContextAsync();

                        HttpListenerRequest request = context.Request;
                        // Obtain a response object.
                        HttpListenerResponse response = context.Response;
                        // Construct a response.
                        string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                        // Get a response stream and write the response to it.
                        response.ContentLength64 = buffer.Length;
                        System.IO.Stream output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);
                        // You must close the output stream.
                        output.Close();
                    }
                    else
                        break;
                }
                catch (Exception e)
                {
                    Utils.PrintMessage(e.Message, LogMessageType.Error);
                }
            }
        }

        public void Stop()
        {
            serverListener.Stop();

            serverStateRunning = ServerState.Stopped;
            Utils.PrintMessage("WebServer stopped...", LogMessageType.Error);
        }

        public void Initialize(string address, string port, string rootDirectory)
        {
            if (!HttpListener.IsSupported)
            {
                Utils.PrintMessage("Windows XP SP2 or Server 2003 is required to use the HttpListener class!", LogMessageType.Error);
                return;
            }

            if (serverStateRunning == ServerState.Started)
            {
                Utils.PrintMessage("WebServer is running! Please stop WebServer...", LogMessageType.Error);
                return;
            }

            string host = Utils.ParseAddressPort(address, port);
            string directory = Utils.ParseDirectoryName(rootDirectory);

            if (host.Trim().Equals("") || directory.Trim().Equals(""))
            {
                Utils.PrintMessage("Invalid input parameters...", LogMessageType.Error);
                return;
            }

            this.rootDirectory = rootDirectory;
            this.host = new Uri(host);

            try
            {
                serverListener = new HttpListener();
                serverListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                serverListener.Prefixes.Add(host);

                serverStateInitialized = ServerState.Initialized;
                Utils.PrintMessage("WebServer initialized...");
            }
            catch (Exception e)
            {
                Utils.PrintMessage(e.Message, LogMessageType.Error);
            }
        }





    }
}
