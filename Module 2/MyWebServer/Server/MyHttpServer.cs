using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebServer
{
    public class MyHttpServer
    {
        private string baseDirectory;
        private string rootDirectory;
        private Uri host;

        private HttpListener serverListener;

        public MyHttpServer(Uri address, string directoryBase, string directoryRoot)
        {
            baseDirectory = directoryBase;
            rootDirectory = directoryRoot;
            host = address;

            serverListener = new HttpListener();
            serverListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            serverListener.Prefixes.Add(address.AbsoluteUri);
        }

        public void Start()
        {
            serverListener.Start();

            Utils.PrintMessage("WebServer started...");
            ListenProcess();
        }

        public void Stop()
        {
            serverListener.Abort();
            serverListener.Stop();

            Utils.PrintMessage("WebServer stopped...");
        }

        private void ListenProcess()
        {
            while (true)
            {
                try
                {
                    HttpListenerContext context = (serverListener.GetContextAsync()).Result;

                    HttpListenerRequest request = context.Request;

                    // Формируем ответ на запрос клиента
                    StringBuilder strB = new StringBuilder();
                    strB.Append("Request Method: ");
                    strB.AppendLine(context.Request.HttpMethod);
                    strB.Append("Request URL: ");
                    strB.AppendLine(context.Request.Url.AbsoluteUri);
                    Utils.PrintMessage(strB.ToString());

                    FileProcess(context);
                }
                catch (Exception e)
                {
                    Utils.PrintMessage(e.Message, LogMessageType.Error);
                }
            }
        }

        private void FileProcess(HttpListenerContext context)
        {
            context.Response.ContentEncoding = Encoding.UTF8;

            var file = context.Request.RawUrl.Substring(1);
            if (String.IsNullOrEmpty(file) || file.EndsWith("/"))
            {
                foreach (string indexFile in Utils.DefaultFiles)
                {
                    file = String.Concat(rootDirectory, @"\", indexFile).Replace("/", @"\");
                    if (File.Exists(file))
                        break;
                }
            }
            else 
            {
                if (!File.Exists(String.Concat(rootDirectory, @"\", file).Replace("/", @"\")))
                    file = String.Concat(baseDirectory, @"\", file).Replace("/", @"\");
                else
                    file = String.Concat(rootDirectory, @"\", file).Replace("/", @"\");
            }

            // проверка запрета на скачивание файлов с таким расширением
            foreach (string extFile in Utils.NoAccessFiles)
            {
                if (Path.GetExtension(file).Contains(extFile))
                {
                    Utils.PrintMessage(String.Format("File {0} no access for download...", file), LogMessageType.Error);
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return;
                }
            }

            if (File.Exists(file))
            {
                try
                {
                    // если есть кеширование и файл не изменялся отправляем код = 304
                    //if (context.Request.Headers["if-Modified-Since"] != null && context.Request.Headers["if-Modified-Since"] == File.GetLastWriteTime(file).ToString("r"))
                    //{
                    //    Utils.PrintMessage(String.Format("File {0} is not modified...", file));
                    //    context.Response.StatusCode = (int)HttpStatusCode.NotModified;
                    //    return;
                    //}

                    using (Stream input = new FileStream(file, FileMode.Open))
                    {
                        // добавляем заголовки в ответ
                        string mime;
                        context.Response.ContentType = Utils.MimeTypes.TryGetValue(Path.GetExtension(file), out mime)
                            ? mime
                            : "application/octet-stream";
                        context.Response.ContentLength64 = input.Length;
                        context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                        // параметры кеширования для браузера                        
                        context.Response.AddHeader("Last-Modified", File.GetLastWriteTime(file).ToString("r"));
                        context.Response.AddHeader("max-age", "86400"); //1 день

                        // контент ответа
                        byte[] buffer = new byte[1024 * 32];
                        int nbytes;
                        while ((nbytes = input.Read(buffer, 0, buffer.Length)) > 0)
                            context.Response.OutputStream.Write(buffer, 0, nbytes);
                        input.Close();
                        Utils.PrintMessage(String.Format("Write file {0} into Response...", file));
                    }
                    context.Response.OutputStream.Flush();
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                }
                catch (Exception e)
                {
                    Utils.PrintMessage(String.Format("{0}...", e.Message), LogMessageType.Error);
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }
            }
            else
            {
                Utils.PrintMessage(String.Format("File {0} not found...", file), LogMessageType.Error);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }

            context.Response.OutputStream.Close();
        }

    }
}
