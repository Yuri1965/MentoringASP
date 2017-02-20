using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CsQuery;

namespace RipperManagerLib
{
    public class RipperManager
    {
        private string siteLink = "";

        private string outputDir = "";

        private DownloadOptionDomain domainOption;
        private DownloadOptionLevel levelOption;

        //ТОЛЬКО ДЛЯ ФАЙЛОВ (картинки, скрипты, css и прочее)!!!!!
        //пара ключ(link на ресурс из html текста) - значение(полный путь и имя файла из локальной директории в которую сохранили);
        private Dictionary<string, string> listLinkAndFile = new Dictionary<string, string>();

        //ТОЛЬКО ДЛЯ ФАЙЛОВ СТРАНИЦ HTML!!!!!
        //пара ключ(link на страницу из html текста) - значение(полный путь и имя файла html страницы из локальной директории);
        private Dictionary<string, string> listLinkHTMLPageAndFile = new Dictionary<string, string>();

        public RipperManager(string siteLink, string outputDir, DownloadOptionDomain domainOption, DownloadOptionLevel levelOption)
        {
            this.siteLink = siteLink;

            this.outputDir = outputDir;

            this.domainOption = domainOption;
            this.levelOption = levelOption;
        }

        public void SetNewOptions(string siteLink, string outputDir, DownloadOptionDomain domainOption, DownloadOptionLevel levelOption)
        {
            this.siteLink = siteLink;

            this.outputDir = outputDir;

            this.domainOption = domainOption;
            this.levelOption = levelOption;

            listLinkAndFile.Clear();
            listLinkHTMLPageAndFile.Clear();
        }

        private string GetFileName(string link, int level, bool isPage = false)
        {
            // вычисляем директорию для сохранения файла
            string saveFileDir = outputDir;
            if (level > 0)
                saveFileDir = String.Concat(saveFileDir, String.Format(@"{0}\", level.ToString()));

            if (!isPage)
                saveFileDir = String.Concat(saveFileDir, @"Files\");

            // получаем имя файла
            string fileName = UtilsLib.GetFileNameFromUrl(link).Trim();

            // если НЕ смогли определить имя файла, то генерим сами
            if (fileName.Equals(""))
                fileName = Guid.NewGuid().ToString();

            fileName = String.Concat(saveFileDir, fileName);
            return fileName;
        }

        private async Task<string> SaveStreamFile(HttpClient client, string link, int level)
        {
            string result = "";
            try
            {
                using (var resp = await client.GetAsync(new Uri(link)))
                {
                    //если ресурс доступен
                    if (resp.IsSuccessStatusCode)
                    {
                        using (Stream content = await resp.Content.ReadAsStreamAsync())
                        {
                            // получаем имя файла
                            string fileName = GetFileName(link, level);

                            string tmpFile = "";
                            if (!listLinkAndFile.TryGetValue(link, out tmpFile))
                            {
                                // если такой файл уже есть, то изменим ему имя
                                FileInfo fileInf = new FileInfo(fileName);
                                if (fileInf.Exists)
                                {
                                    string fileWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                                    string newFileName = fileWithoutExt + Guid.NewGuid().ToString();
                                    fileName = fileName.Replace(fileWithoutExt, newFileName);
                                }

                                UtilsLib.SaveFile(fileName, content);
                                listLinkAndFile.Add(link, fileName);

                                result = fileName;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("This address is {0} is not available or an error occurred. Query returned StatusCode = {1}", link, resp.StatusCode);
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }

        private async Task<string> SaveTextFile(HttpClient client, string link, int level)
        {
            string result = "";
            try
            {
                using (var resp = await client.GetAsync(new Uri(link)))
                {
                    //если ресурс доступен
                    if (resp.IsSuccessStatusCode)
                    {
                        // получаем контент
                        string content = await resp.Content.ReadAsStringAsync();

                        // получаем имя файла
                        string fileName = GetFileName(link, level);

                        string tmpFile = "";
                        if (!listLinkAndFile.TryGetValue(link, out tmpFile))
                        {
                            // если такой файл уже есть, то изменим ему имя
                            FileInfo fileInf = new FileInfo(fileName);
                            if (fileInf.Exists)
                            {
                                string fileWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                                string newFileName = fileWithoutExt + Guid.NewGuid().ToString();
                                fileName = fileName.Replace(fileWithoutExt, newFileName);
                            }

                            UtilsLib.SaveFile(fileName, content);
                            listLinkAndFile.Add(link, fileName);

                            result = fileName;
                        }
                    }
                    else
                    {
                        Console.WriteLine("This address is {0} is not available or an error occurred. Query returned StatusCode = {1}", link, resp.StatusCode);
                    }

                    return result;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }

        private async Task<string> SaveHTMLPage(HttpClient client, string link, int level, bool firstPage = false)
        {
            string result = "";
            try
            {
                using (var resp = await client.GetAsync(new Uri(link)))
                {
                    //если ресурс доступен
                    if (resp.IsSuccessStatusCode)
                    {
                        // получаем контент
                        string content = await resp.Content.ReadAsStringAsync();
                        string realLink = resp.RequestMessage.RequestUri.AbsoluteUri;

                        // парсим контент и перебиваем ссылки
                        result = ParseHTMLContent(realLink, content, level, firstPage);
                    }
                    else
                    {
                        Console.WriteLine("This address is {0} is not available or an error occurred. Query returned StatusCode = {1}", link, resp.StatusCode);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }

        private ContentTypes GetContentType(string contType)
        {
            ContentTypes result = ContentTypes.ContentNotSupported;

            if (contType.Contains("text/css") || contType.Contains("application/javascript") || contType.Contains("text/javascript") ||
                contType.Contains("application/json") || contType.Contains("application/xml") ||
                contType.Contains("text/csv"))
                result = ContentTypes.ContentText;

            if (contType.Contains("text/html") || contType.Contains("application/xhtml+xml"))
                result = ContentTypes.ContentTextHTML;

            if (contType.Contains("application/octet-stream") || contType.Contains("image/gif") ||
                contType.Contains("image/x-icon") || contType.Contains("image/jpeg") || contType.Contains("application/pdf") ||
                contType.Contains("image/svg+xml") || contType.Contains("image/tiff") || contType.Contains("image/png"))
                result = ContentTypes.ContentStream;

            return result;
        }

        private async Task<string> SaveResource(string link, int level, bool firstPage = false)
        {
            string result = "";

            //если уровень уже превышен то НЕ сохраняем дальше
            if (!firstPage && (int)levelOption < level)
                return result;

            try
            {
                Console.WriteLine("Information request to the address: {0}", link);

                HttpClient client = new HttpClient();

                ContentTypes contentType = ContentTypes.ContentNotSupported;

                // сначала проверим тип содержимого которое находится по адресу
                using (HttpRequestMessage requestHead = new HttpRequestMessage(HttpMethod.Head, new Uri(link)))
                {
                    using (HttpResponseMessage responseHead = await client.SendAsync(requestHead))
                    {
                        //если ресурс НЕ доступен, то сразу выходим
                        if (!responseHead.IsSuccessStatusCode)
                        {
                            Console.WriteLine("This address is {0} is not available or an error occurred. Query returned StatusCode = {1}",
                                                link, responseHead.StatusCode);

                            return result;
                        }

                        //определим тип содержимого
                        contentType = GetContentType(responseHead.Content.Headers.ContentType.MediaType);

                        // страница HTML
                        if (contentType == ContentTypes.ContentTextHTML)
                        {
                            // если качаем страницы только текущего домена, а url запроса ссылается на другой домен, то НЕ сохраняем
                            if ((domainOption == DownloadOptionDomain.Domain) && (!link.Contains(siteLink)))
                                return result;

                            result = await SaveHTMLPage(client, link, level, firstPage);

                            return result;
                        }

                        // скрипты, css и прочие файлы
                        if (contentType == ContentTypes.ContentText)
                        {
                            result = await SaveTextFile(client, link, level);

                            return result;
                        }

                        if (contentType == ContentTypes.ContentStream)
                        {
                            result = await SaveStreamFile(client, link, level);

                            return result;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }

        private string ParseHTMLContent(string realLink, string content, int level, bool firstPage = false)
        {
            string saveFile = "";

            try
            {
                var dom = new CQ(content);

                // вычисляем директорию для сохранения файла
                string saveFileDir = outputDir;
                // получаем имя файла
                string fileName = UtilsLib.GetFileNameFromUrl(realLink).Trim();
                // если НЕ смогли определить, то генерим сами
                if (fileName.Equals(""))
                {
                    string guidStr = Guid.NewGuid().ToString();
                    fileName = String.Concat(guidStr, ".html");
                }
                if (Path.GetExtension(fileName).Equals(""))
                    fileName = String.Concat(fileName, ".html");

                if (level > 0)
                    saveFileDir = String.Concat(saveFileDir, String.Format(@"{0}\", level.ToString()));

                // сразу добавим страницу в словарь, иначе зациклимся
                string tmpDic = "";
                if (!listLinkHTMLPageAndFile.TryGetValue(realLink, out tmpDic))
                {
                    listLinkHTMLPageAndFile.Add(realLink, fileName);
                }

                // обработаем контент html страницы
                // ищем ссылки на файлы контента по link
                foreach (IDomObject obj in dom.Find("link"))
                {
                    string linkFile = obj.GetAttribute("href");
                    if (linkFile == null)
                        continue;

                    string typeFile = obj.GetAttribute("rel");

                    if (typeFile != null && typeFile.Trim().Equals("stylesheet", StringComparison.OrdinalIgnoreCase))
                    {
                        if (linkFile != null && !linkFile.Contains("http://") && !linkFile.Contains("https://"))
                            linkFile = String.Concat(siteLink, linkFile);

                        string tmp = "";
                        if (!listLinkAndFile.TryGetValue(linkFile, out tmp))
                        {
                            string fileSaved = (SaveResource(linkFile, level, firstPage)).Result;

                            if (!fileSaved.Trim().Equals(""))
                                obj.SetAttribute("href", new Uri(fileSaved).AbsoluteUri.Trim());
                        }
                    }
                }

                // ищем ссылки на файлы контента по script
                foreach (IDomObject obj in dom.Find("script"))
                {
                    string linkFile = obj.GetAttribute("src");
                    if (linkFile == null)
                        continue;

                    if (!linkFile.Contains("http://") && !linkFile.Contains("https://"))
                        linkFile = String.Concat(siteLink, linkFile);

                    string tmp = "";
                    if (!listLinkAndFile.TryGetValue(linkFile, out tmp))
                    {
                        string fileSaved = (SaveResource(linkFile, level, firstPage)).Result;

                        if (!fileSaved.Trim().Equals(""))
                            obj.SetAttribute("src", new Uri(fileSaved).AbsoluteUri.Trim());
                    }
                }

                // ищем ссылки на файлы контента по img
                foreach (IDomObject obj in dom.Find("img"))
                {
                    string linkFile = obj.GetAttribute("src");
                    if (linkFile == null)
                        continue;

                    string tmpSplitter = linkFile.Substring(0, 2);

                    if (!linkFile.Contains("http://") && !linkFile.Contains("https://") && tmpSplitter.Equals("//"))
                        linkFile = String.Concat("http:", linkFile);
                    else
                    {
                        tmpSplitter = linkFile.Substring(0, 1);

                        if (!linkFile.Contains("http://") && !linkFile.Contains("https://") && tmpSplitter.Equals("/"))
                            linkFile = String.Concat(siteLink, linkFile);
                    }

                    if (Path.GetExtension(UtilsLib.GetFileNameFromUrl(linkFile).Trim()).Equals(""))
                        continue;

                    string tmp = "";
                    if (!listLinkAndFile.TryGetValue(linkFile, out tmp))
                    {
                        string fileSaved = (SaveResource(linkFile, level, firstPage)).Result;

                        if (!fileSaved.Trim().Equals(""))
                            obj.SetAttribute("src", new Uri(fileSaved).AbsoluteUri.Trim());
                    }
                }

                // ищем ссылки на файлы контента по a
                foreach (IDomObject obj in dom.Find("a"))
                {
                    string linkFile = obj.GetAttribute("href");
                    if (linkFile == null || linkFile.Contains("#"))
                        continue;

                    string classFile = obj.GetAttribute("class");

                    if (!linkFile.Contains("http://") && !linkFile.Contains("https://"))
                        linkFile = String.Concat(siteLink, linkFile);

                    string tmp = "";
                    if (!listLinkAndFile.TryGetValue(linkFile, out tmp))
                    {
                        // если это не картинка, то это страница поэтому проверим ее в нашем словаре,
                        // и еcли она уже есть то сразу перебьем ссылку
                        if (classFile == null || !classFile.Trim().Equals("image"))
                        {
                            string fileDic = "";
                            if (listLinkHTMLPageAndFile.TryGetValue(linkFile, out fileDic))
                            {
                                string fileFromDic = new Uri(fileDic).AbsoluteUri;
                                obj.SetAttribute("href", fileFromDic.Trim());
                                continue;
                            }

                            firstPage = false;
                            if ((int)levelOption >= level + 1)
                                level = level + 1;
                            else continue;
                        }

                        string fileSaved = (SaveResource(linkFile, level, firstPage)).Result;

                        if (!fileSaved.Trim().Equals(""))
                            obj.SetAttribute("href", new Uri(fileSaved).AbsoluteUri.Trim());
                    }
                }

                string tmpFile = "";
                if (listLinkHTMLPageAndFile.TryGetValue(realLink, out tmpFile))
                {
                    dom.Save(fileName);

                    if (listLinkHTMLPageAndFile.ContainsKey(realLink))
                        listLinkHTMLPageAndFile.Remove(realLink);

                    listLinkHTMLPageAndFile.Add(realLink, fileName);
                }

                saveFile = fileName;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return saveFile;
        }

        public void SaveSite()
        {
            string saveFile = (SaveResource(siteLink, 0, true)).Result;
        }
    }
}
