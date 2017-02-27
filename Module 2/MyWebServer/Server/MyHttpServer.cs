using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace WebServer
{
    public class MyHttpServer
    {
        private readonly Uri baseDirectory;
        private readonly Uri rootDirectory;
        private readonly Uri host;

        private readonly HttpListener serverListener;

        public MyHttpServer(Uri address, Uri directoryBase, Uri directoryRoot)
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
                    context.Response.Close();
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

            Uri file;
            var requestUrl = context.Request.Url;
            if (requestUrl.Segments.Last().EndsWith("/"))
            {
                foreach (string indexFile in Utils.DefaultFiles)
                {
                    file = new Uri(rootDirectory, new Uri(indexFile, UriKind.Relative));
                    if (File.Exists(file.LocalPath))
                        break;
                }
                context.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }
            else 
            {
                if (!File.Exists(new Uri(rootDirectory, requestUrl.MakeRelativeUri(host)).LocalPath))
                    file = new Uri(baseDirectory, host.MakeRelativeUri(requestUrl));
                else
                    file = new Uri(rootDirectory, host.MakeRelativeUri(requestUrl));
            }

            var filePath = file.LocalPath;
            // проверка запрета на скачивание файлов с таким расширением
            foreach (string extFile in Utils.NoAccessFiles)
            {
                if (Path.GetExtension(filePath).Contains(extFile))
                {
                    Utils.PrintMessage(String.Format("File {0} no access for download...", file), LogMessageType.Error);
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return;
                }
            }

            if (File.Exists(filePath))
            {
                try
                {
                    // если есть кеширование и файл не изменялся отправляем код = 304
                    if (context.Request.Headers["if-Modified-Since"] != null && context.Request.Headers["if-Modified-Since"] == File.GetLastWriteTime(filePath).ToString("r"))
                    {
                        Utils.PrintMessage(String.Format("File {0} is not modified...", file));
                        context.Response.StatusCode = (int)HttpStatusCode.NotModified;
                        return;
                    }

                    using (Stream input = new FileStream(filePath, FileMode.Open))
                    {
                        // добавляем заголовки в ответ
                        string mime;
                        context.Response.ContentType = Utils.MimeTypes.TryGetValue(Path.GetExtension(filePath), out mime)
                            ? mime
                            : "application/octet-stream";
                        context.Response.ContentLength64 = input.Length;
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                        // параметры кеширования для браузера                        
                        context.Response.AddHeader("Last-Modified", File.GetLastWriteTime(filePath).ToString("r"));
                        context.Response.AddHeader("max-age", "86400"); //1 день

                        // контент ответа
                        input.CopyTo(context.Response.OutputStream);
                        Utils.PrintMessage(String.Format("Write file {0} into Response...", file));
                    }
                    context.Response.OutputStream.Flush();
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
