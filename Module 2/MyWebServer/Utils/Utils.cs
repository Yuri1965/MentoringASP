﻿using System;
using System.Collections.Generic;
using System.IO;

namespace WebServer
{
    public static class Utils
    {
        // Коллекция Mime Type
        private static IDictionary<string, string> mimeTypes =
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                #region extension to MIME type list
                {".asf", "video/x-ms-asf"},
                {".asx", "video/x-ms-asf"},
                {".avi", "video/x-msvideo"},
                {".bin", "application/octet-stream"},
                {".cco", "application/x-cocoa"},
                {".crt", "application/x-x509-ca-cert"},
                {".css", "text/css"},
                {".deb", "application/octet-stream"},
                {".der", "application/x-x509-ca-cert"},
                {".dll", "application/octet-stream"},
                {".dmg", "application/octet-stream"},
                {".ear", "application/java-archive"},
                {".eot", "application/octet-stream"},
                {".exe", "application/octet-stream"},
                {".flv", "video/x-flv"},
                {".gif", "image/gif"},
                {".hqx", "application/mac-binhex40"},
                {".htc", "text/x-component"},
                {".htm", "text/html"},
                {".html", "text/html"},
                {".ico", "image/x-icon"},
                {".img", "application/octet-stream"},
                {".iso", "application/octet-stream"},
                {".jar", "application/java-archive"},
                {".jardiff", "application/x-java-archive-diff"},
                {".jng", "image/x-jng"},
                {".jnlp", "application/x-java-jnlp-file"},
                {".jpeg", "image/jpeg"},
                {".jpg", "image/jpeg"},
                {".js", "application/x-javascript"},
                {".mml", "text/mathml"},
                {".mng", "video/x-mng"},
                {".mov", "video/quicktime"},
                {".mp3", "audio/mpeg"},
                {".mpeg", "video/mpeg"},
                {".mpg", "video/mpeg"},
                {".msi", "application/octet-stream"},
                {".msm", "application/octet-stream"},
                {".msp", "application/octet-stream"},
                {".pdb", "application/x-pilot"},
                {".pdf", "application/pdf"},
                {".pem", "application/x-x509-ca-cert"},
                {".pl", "application/x-perl"},
                {".pm", "application/x-perl"},
                {".png", "image/png"},
                {".prc", "application/x-pilot"},
                {".ra", "audio/x-realaudio"},
                {".rar", "application/x-rar-compressed"},
                {".rpm", "application/x-redhat-package-manager"},
                {".rss", "text/xml"},
                {".run", "application/x-makeself"},
                {".sea", "application/x-sea"},
                {".shtml", "text/html"},
                {".sit", "application/x-stuffit"},
                {".swf", "application/x-shockwave-flash"},
                {".tcl", "application/x-tcl"},
                {".tk", "application/x-tcl"},
                {".txt", "text/plain"},
                {".war", "application/java-archive"},
                {".wbmp", "image/vnd.wap.wbmp"},
                {".wmv", "video/x-ms-wmv"},
                {".xml", "text/xml"},
                {".xpi", "application/x-xpinstall"},
                {".zip", "application/zip"},

                #endregion
            };

        // Коллекция Mime Type
        public static IDictionary<string, string> MimeTypes { get { return mimeTypes; } }

        // массив названий файлов, которые по умолчанию будем искать в директории
        // для случаев когда в URL не указано конкретное имя страницы html страницы
        public static string[] DefaultFiles =
        {
            "index.html",
            "index.htm",
            "default.html",
            "default.htm"
        };

        // массив расширений файлов, к которым запрещен доступ с браузера
        public static string[] NoAccessFiles =
        {
            "dll",
            "config"
        };

        // парсит и возвращает IP + порт в виде строки, если НЕ верно введены данные, то возвращает null
        public static Uri ParseAddressPort(string strAddress, string strPort)
        {
            Uri result = null;

            int port = 0;
            if (!Int32.TryParse(strPort, out port))
                return result;

            try
            {
                UriBuilder myURI = new UriBuilder("http", strAddress, port);
                result = new Uri(myURI.Uri.AbsoluteUri);
            }
            catch (Exception e)
            {
                PrintMessage(e.Message, LogMessageType.Error);
            }
            return result;
        }

        // для вывода сообщений в консоль (логирование)
        public static void PrintMessage(string message, LogMessageType messageType = LogMessageType.Info)
        {
            if (messageType == LogMessageType.Info)
                Console.Write("WebServer info: {0}\n", message);
            else
                if (messageType == LogMessageType.Error)
                    Console.Write("WebServer error: {0}\n", message);
        }

        // получает имя директории в виде Uri
        public static Uri GetUriFromPath(string path)
        {
            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()) && !path.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
                path += Path.DirectorySeparatorChar;

            return new Uri(Path.GetFullPath(path));
        }
    }
}
