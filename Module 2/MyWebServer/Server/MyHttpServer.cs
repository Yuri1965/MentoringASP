using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace WebServer
{
    public class MyHttpServer
    {
        // базовый директорий где лежат файлы доменов
        private readonly Uri baseDirectory;
        // стартовый директорий - начальная страница в браузере
        private readonly Uri rootDirectory;
        // адрес хоста с портом
        private readonly Uri host;

        // прослушиватель IP и порта
        private readonly HttpListener serverListener;

        public MyHttpServer(Uri address, Uri directoryBase, Uri directoryRoot)
        {
            baseDirectory = directoryBase;
            rootDirectory = directoryRoot;
            host = address;

            // создаем прослушиватель HttpListener
            serverListener = new HttpListener();
            serverListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            // это добавление IP и порта для прослушки
            serverListener.Prefixes.Add(address.AbsoluteUri);
        }

        public void Start()
        {
            serverListener.Start();

            Utils.PrintMessage("WebServer started...");
            ListenProcess();
        }

        //метод не используется в данной реализации, сделан просто для полноты работы класса
        public void Stop()
        {
            serverListener.Abort();
            serverListener.Stop();

            Utils.PrintMessage("WebServer stopped...");
        }

        // метод прослушки входящих запросов от клиента - браузера
        private void ListenProcess()
        {
            // цикл прослушки
            while (true)
            {
                try
                {
                    // получает запрос от клиента
                    HttpListenerContext context = (serverListener.GetContextAsync()).Result;

                    // просто выводим в лог, информацию о запросе, который пришел с клиента
                    // можно вынести в отдельный приватный метод, если надо добавить более подробную информацию
                    StringBuilder strB = new StringBuilder();
                    strB.Append("Request Method: ");
                    strB.AppendLine(context.Request.HttpMethod);
                    strB.Append("Request URL: ");
                    strB.AppendLine(context.Request.Url.AbsoluteUri);
                    Utils.PrintMessage(strB.ToString());

                    // Формируем ответ на запрос клиента
                    FileProcess(context);
                    // Работа с context.Response закончена и надо закрыть, чтобы ответ нормально ушел клиенту
                    context.Response.Close();
                }
                catch (Exception e)
                {
                    Utils.PrintMessage(e.Message, LogMessageType.Error);
                }
            }
        }

        // собирает из параметров полный путь к файлу и проверяет его наличие
        private Uri CheckUriFileNameFound(Uri directoryName, Uri urlPath, string fileName = "")
        {
            string pathUrl = urlPath.LocalPath;
            if (pathUrl.StartsWith(@"/"))
                pathUrl = pathUrl.Remove(0, 1);

            var file = new Uri(String.Concat(directoryName.LocalPath, pathUrl, fileName));
            if (!File.Exists(file.LocalPath))
                file = null;
            return file;
        }

        //метод обработки запроса клиента и формирования ответа на запрос
        private void FileProcess(HttpListenerContext context)
        {
            context.Response.ContentEncoding = Encoding.UTF8;

            // определение пути и имени файла ресурса домена, который будем отдавать в качестве ответа клиенту
            Uri file = null;
            var requestUrl = context.Request.Url;
            if (requestUrl.Segments.Last().EndsWith("/"))
            {
                foreach (string indexFile in Utils.DefaultFiles)
                {
                    file = CheckUriFileNameFound(rootDirectory, requestUrl, indexFile);
                    if (file == null)
                        file = CheckUriFileNameFound(baseDirectory, requestUrl, indexFile);

                    if (file != null && File.Exists(file.LocalPath))
                        break;
                }
            }
            else
            {
                file = CheckUriFileNameFound(rootDirectory, requestUrl);
                if (file == null)
                    file = CheckUriFileNameFound(baseDirectory, requestUrl);
            }

            // если файл НЕ найден, то отправляем в ответ код = 404
            if (file == null)
            {
                // отправим клиенту код = 404 - ресурс не найден
                Utils.PrintMessage(String.Format("File {0} not found...", file), LogMessageType.Error);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            var filePath = file.LocalPath;
            // проверка запрета на скачивание файлов с определенным расширением
            foreach (string extFile in Utils.NoAccessFiles)
            {
                if (Path.GetExtension(filePath).Contains(extFile))
                {
                    Utils.PrintMessage(String.Format("File {0} no access for download...", file), LogMessageType.Error);
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return;
                }
            }

            // формируем ответ клиенту
            try
            {
                // если есть кеширование и файл не изменялся отправляем код = 304
                if (context.Request.Headers["if-Modified-Since"] != null && context.Request.Headers["if-Modified-Since"] == File.GetLastWriteTime(filePath).ToString("r"))
                {
                    Utils.PrintMessage(String.Format("File {0} is not modified...", file));
                    context.Response.StatusCode = (int)HttpStatusCode.NotModified;
                    return;
                }

                // получаем содержимое файла и пишем это содержимое в context.Response.OutputStream
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

                    // пишем файл в context.Response.OutputStream
                    Utils.PrintMessage(String.Format("Write file {0} into Response...", file));
                    input.CopyTo(context.Response.OutputStream);

                    // чистим все буферы в памяти для Response.OutputStream
                    context.Response.OutputStream.Flush();
                    // закрываем поток записи Response.OutputStream
                    context.Response.OutputStream.Close();
                }
            }
            catch (Exception e)
            {
                // ошибки подробно обрабатывать НЕ будем, отправим что сервер не смог сформировать ответ на запрос клиента
                Utils.PrintMessage(String.Format("{0}...", e.Message), LogMessageType.Error);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }

    }
}
