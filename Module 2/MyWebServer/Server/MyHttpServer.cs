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

        private HttpListener serverListener;

        public MyHttpServer(Uri address, string directory)
        {
            rootDirectory = directory;
            host = address;

            serverListener = new HttpListener();
            serverListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            serverListener.Prefixes.Add(address.AbsoluteUri);
        }

        public void Start()
        {
            serverListener.Start();

            Utils.PrintMessage("WebServer started...", LogMessageType.Error);
            ListenProcess();
        }

        public void Stop()
        {
            serverListener.Abort();
            serverListener.Stop();

            Utils.PrintMessage("WebServer stopped...", LogMessageType.Error);
        }

        private void ListenProcess()
        {
            while (true)
            {
                try
                {
                    HttpListenerContext context = (serverListener.GetContextAsync()).Result;

                    HttpListenerRequest request = context.Request;
                    // Obtain a response object.
                    using (HttpListenerResponse response = context.Response)
                    {
                        StringBuilder strB = new StringBuilder();
                        strB.Append("Request Method: ");
                        strB.AppendLine(request.HttpMethod);
                        strB.Append("Request URL: ");
                        strB.AppendLine(request.Url.AbsoluteUri);
                        Utils.PrintMessage(strB.ToString());

                        string reqMetod = request.HttpMethod;
                        string respAbsUri = request.Url.AbsoluteUri;
                        string respAbsUrl = request.Url.AbsolutePath;

                        var respQuery = request.QueryString;

                        response.ContentType = "text/html";
                        response.ContentEncoding = Encoding.UTF8;
                        response.StatusCode = 200;

                        strB.Clear();
                        // Construct a response.
                        foreach (var param in respQuery)
                        {
                            strB.Append(request.QueryString[(string)param]);
                        }

                        byte[] bytes = Encoding.Default.GetBytes(strB.ToString());
                        strB.Clear();
                        strB.Append(Encoding.UTF8.GetString(bytes));

                        string responseString = "<HTML><BODY><b>" + strB + "</b></BODY></HTML>";

                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                        // Get a response stream and write the response to it.
                        response.ContentLength64 = buffer.Length;
                        using (System.IO.Stream output = response.OutputStream)
                        {
                            output.Write(buffer, 0, buffer.Length);
                            // You must close the output stream.
                            output.Close();
                        }
                    }
                }
                catch (Exception e)
                {
                    Utils.PrintMessage(e.Message, LogMessageType.Error);
                }
            }
        }



    }
}
