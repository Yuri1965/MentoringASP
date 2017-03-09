using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using AngleSharp.Parser.Html;

namespace Eget
{
    public enum UriFollowRestrictions
    {
        NoRestrictions,
        FromSameHost,
        FromSuburl
    }

    public class CrawlerOptions
    {
        public CrawlerOptions(int? maxDepth, string outputDirectory, bool shouldFollowPageLinks, bool shouldFollowResourceLinks,
            UriFollowRestrictions followRestrictions, bool enableLogging)
        {
            Debug.Assert(maxDepth == null || maxDepth >= 0, "maxDepth cannot be negative");
            MaxDepth = maxDepth;
            ShouldFollowPageLinks = shouldFollowPageLinks;
            ShouldFollowResourceLinks = shouldFollowResourceLinks;
            FollowRestrictions = followRestrictions;
            EnableLogging = enableLogging;

            if (!outputDirectory.EndsWith(Path.DirectorySeparatorChar.ToString())
                && !outputDirectory.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
            {
                outputDirectory += Path.DirectorySeparatorChar;
            }
            OutputDirectory = new Uri(Path.GetFullPath(outputDirectory));
        }

        public int? MaxDepth { get; private set; }
        public bool ShouldFollowPageLinks { get; private set; }
        public bool ShouldFollowResourceLinks { get; private set; }
        public UriFollowRestrictions FollowRestrictions { get; private set; }
        public bool EnableLogging { get; private set; }
        public Uri OutputDirectory { get; private set; }
    }

    public class Crawler
    {
        private readonly Dictionary<Uri, Uri> _localFileNames = new Dictionary<Uri, Uri>();
        private readonly Queue<Tuple<Uri, int>> _prioritizedQueuedUris = new Queue<Tuple<Uri, int>>();
        private readonly Queue<Tuple<Uri, int>> _queuedUris = new Queue<Tuple<Uri, int>>();
        private readonly HashSet<Uri> _takenFilenames = new HashSet<Uri>();
        private readonly CrawlerOptions _options;
        private Uri _baseUri = null;
        private int _tempFileCounter = 0;

        private HttpClient client = new HttpClient();

        public Crawler(CrawlerOptions options)
        {
            _options = options;
        }

        public void DownloadPage(Uri address)
        {
            Debug.Assert(_baseUri == null, "_baseUri == null");
            _baseUri = address;
            try
            {
                QueuePage(address, 0, false);
                DownloadAll();
            }
            finally
            {
                _baseUri = null;
            }
        }

        private void DownloadAll()
        {
            while (_queuedUris.Count > 0 || _prioritizedQueuedUris.Count > 0)
            {
                var addressAndDepth = _prioritizedQueuedUris.Count > 0 ? _prioritizedQueuedUris.Dequeue() : _queuedUris.Dequeue();
                DownloadAndProcess(addressAndDepth.Item1, addressAndDepth.Item2);
            }
        }

        private void DownloadAndProcess(Uri address, int depth)
        {
            LogMessage("Processing {0}", address);

            //var client = new HttpClient();
            //HttpResponseMessage response;
            try
            {
                using (HttpResponseMessage response = client.GetAsync(address).Result)
                {
                    LogMessage("Status {0}", response.StatusCode);
                    if (!response.IsSuccessStatusCode || response.Content == null || response.Content.Headers == null)
                        return;

                    var contentLength = response.Content.Headers.ContentLength;
                    if (contentLength != null)
                        LogMessage("Content-Length is {0}", contentLength);

                    var contentType = response.Content.Headers.ContentType;
                    if (contentType != null)
                        LogMessage("Content-Type is {0}", contentType);

                    HandleDownloadedPage(address, _localFileNames[address], response, depth);
                }
            }
            catch (AggregateException e)
            {
                LogMessage("Couldn't load {0}. Exception fired when loading:\n{1}", address, e);
                return;
            }
        }

        private Uri QueuePage(Uri address, int depth, bool isResourceLink)
        {
            // Проверим можно ли нам пройти по этой ссылке
            if (!isResourceLink)
            {
                switch (_options.FollowRestrictions)
                {
                    case UriFollowRestrictions.FromSameHost:
                        if (address.Host != _baseUri.Host)
                            return null;
                        break;
                    case UriFollowRestrictions.FromSuburl:
                        if (!_baseUri.IsBaseOf(address))
                            return null;
                        break;
                    case UriFollowRestrictions.NoRestrictions:
                        break;
                    default:
                        Debug.Fail("invalid value for UriFollowRestrictions");
                        break;
                }
            }

            // Проверим глубину ссылок
            if (_options.MaxDepth != null && _options.MaxDepth.Value < depth)
                return null;

            // HttpClient может обрабатывать только адреса со схемами http:// и https://, поэтому все остальные просто пропускаем
            if (address.Scheme != Uri.UriSchemeHttp && address.Scheme != Uri.UriSchemeHttps)
                return null;

            Uri localName;;
            if (_localFileNames.TryGetValue(address, out localName))
                return localName;
            localName = DeriveLocalName(address);

            _localFileNames.Add(address, localName);
            _takenFilenames.Add(localName);
            (isResourceLink ? _prioritizedQueuedUris : _queuedUris).Enqueue(Tuple.Create(address, depth));

            return localName;
        }

        private Uri DeriveLocalName(Uri address)
        {
            var localFileUri = DeriveDescriptiveName(address);
            if (!_takenFilenames.Contains(localFileUri) && CheckForValidPathByCreatingFile(localFileUri.LocalPath))
                return localFileUri;
            return GetNextUnusedTempFilename();
        }

        private Uri DeriveDescriptiveName(Uri address)
        {
            // Создадим относительный url вида "<host-name>/<path>"
            var relativePath = address.AbsolutePath;

            // Windows не позволяет использовать некорректные символы, вроде `:` в именах файлов или папок. Заменим его на "_"
            // Здесь же можно было бы обработать другие некорректные символы, вроде '?', '<', '>' и т.д.
            // примерно так: var fileName = string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));
            // но с наскоку это сделать не получилось, поэтому оставил как есть потому что и так работает
            relativePath = relativePath.Replace(":", "_");

            if (relativePath.EndsWith("/"))
                relativePath += "index.html";

            Debug.Assert(relativePath.StartsWith("/"));
            var relativeUri = new Uri(address.Host + relativePath, UriKind.Relative);
            return new Uri(_options.OutputDirectory, relativeUri);
        }

        private bool CheckForValidPathByCreatingFile(string path)
        {
            try
            {
                var directory = Path.GetDirectoryName(path);
                Debug.Assert(directory != null, "directory != null");
                Directory.CreateDirectory(directory);
            }
            catch (ArgumentException)
            {
                return false;
            }
            //catch (PathTooLongException)
            //{
            //    return false;
            //}
            //catch (DirectoryNotFoundException)
            //{
            //    return false;
            //}
            catch (IOException)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }

            try
            {
                using (var openedFile = File.Open(path, FileMode.Create))
                {
                    openedFile.Close();
                    return true;
                }
            }
            catch (PathTooLongException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                // Возвращается в т.ч. когда уже создана папка с таким же именем
                return false;
            }
        }

        private Uri GetNextUnusedTempFilename()
        {
            while (true)
            {
                ++_tempFileCounter;
                var uri = new Uri(Path.Combine(_options.OutputDirectory.LocalPath, "tmpFiles\\tmpfile_" + _tempFileCounter));
                if (!_takenFilenames.Contains(uri))
                    return uri;
            }
        }

        private void HandleDownloadedPage(Uri address, Uri localAddress, HttpResponseMessage response, int depth)
        {
            using (var input = response.Content.ReadAsStreamAsync().Result)
            {
                var contentType = response.Content.Headers.ContentType;
                if (MustParseAndProcessHtml() && contentType != null && contentType.MediaType.EqualsIgnoringCase("text/html"))
                    HandleHtml(address, localAddress, input, depth);
                else
                    SaveLocally(localAddress, input);
            }
        }

        private bool MustParseAndProcessHtml()
        {
            return _options.ShouldFollowPageLinks || _options.ShouldFollowResourceLinks;
        }

        private void SaveLocally(Uri localAddress, Stream result)
        {
            using (var outputStream = OpenOutputStream(localAddress))
                result.CopyTo(outputStream);
        }

        private Stream OpenOutputStream(Uri localAddress)
        {
            Debug.Assert(localAddress.IsAbsoluteUri, "localAddresses must be absolute");
            Debug.Assert(localAddress.Scheme == Uri.UriSchemeFile, "localAddresses must have file scheme");
            var absolutePath = localAddress.LocalPath;

            Directory.CreateDirectory(Path.GetDirectoryName(absolutePath));
            return new FileStream(absolutePath, FileMode.Create);
        }

        private void HandleHtml(Uri address, Uri localAddress, Stream input, int depth)
        {
            var parser = new HtmlParser(new HtmlParserOptions {IsEmbedded = false, IsScripting = false, IsStrictMode = false});
            var document = parser.Parse(input);

            QueueAndReplaceLinks(address, localAddress, document, depth);
            using (var writer = new StreamWriter(OpenOutputStream(localAddress)))
                document.ToHtml(writer);
        }

        private void QueueAndReplaceLinks(Uri address, Uri localAddress, IHtmlDocument document, int depth)
        {
            foreach (var element in document.All)
            {
                bool isResourceLink;
                string attributeName;
                var linkUri = TryExtractLinkUriFromElement(address, element, out isResourceLink, out attributeName);
                if (linkUri == null)
                    continue;

                if (isResourceLink && !_options.ShouldFollowResourceLinks)
                    continue;

                if (!isResourceLink && !_options.ShouldFollowPageLinks)
                    continue;

                var linkLocalAddress = QueuePage(linkUri, isResourceLink ? depth : depth + 1, isResourceLink);
                if (linkLocalAddress == null)
                    continue;

                var uri = DeriveLinkReplacement(localAddress, linkLocalAddress);

                element.SetAttribute(attributeName, uri.ToString());
            }
        }

        private Uri DeriveLinkReplacement(Uri baseUri, Uri targetUri)
        {
            Debug.Assert(baseUri.IsAbsoluteUri && baseUri.Scheme == Uri.UriSchemeFile, "Must be an absolute link with file scheme");
            Debug.Assert(targetUri.IsAbsoluteUri && targetUri.Scheme == Uri.UriSchemeFile, "Must be an absolute link with file scheme");
            return baseUri.MakeRelativeUri(targetUri);
        }

        private static Uri TryExtractLinkUriFromElement(Uri address, IElement element, out bool isResourceLink, out string attributeName)
        {
            var linkStr = TryExtractLinkTextFromElement(element, out isResourceLink, out attributeName);
            Uri linkUri;
            if (linkStr == null)
                return null;

            // Сначала попробуем разобрать ссылку как относительную (UriKind.Relative),
            // а если не получилось - как абсолютную(UriKind.Absolute)
            if (!Uri.TryCreate(linkStr, UriKind.Relative, out linkUri) && !Uri.TryCreate(linkStr, UriKind.Absolute, out linkUri))
                return null;

            // Если ссылка была абсолютной - сразу вернём её
            if (linkUri.IsAbsoluteUri) return linkUri;

            // А относительную переделаем в абсолютную
            if (!Uri.TryCreate(address, linkUri, out linkUri))
                return null;
            return linkUri;
        }

        private static string TryExtractLinkTextFromElement(IElement element, out bool isResourceLink, out string attributeName)
        {
            if (element.LocalName.EqualsIgnoringCase("a"))
            {
                isResourceLink = false;
                attributeName = "href";
            }
            else if (element.LocalName.EqualsIgnoringCase("link") && IsResourceLinkRelAttribute(element.GetAttribute("rel")))
            {
                isResourceLink = true;
                attributeName = "href";
            }
            else if (element.LocalName.EqualsIgnoringCase("img") || element.LocalName.EqualsIgnoringCase("script"))
            {
                isResourceLink = true;
                attributeName = "src";
            }
            else
            {
                isResourceLink = false;
                attributeName = null;
                return null;
            }
            return element.GetAttribute(attributeName);
        }

        private static bool IsResourceLinkRelAttribute(string relAttribute)
        {
            return relAttribute == "icon" || relAttribute == "shortcut icon" || relAttribute == "stylesheet";
        }

        private void LogMessage(string message, params object[] args)
        {
            if (!_options.EnableLogging)
                return;
            Console.WriteLine(message, args);
        }
    }
}