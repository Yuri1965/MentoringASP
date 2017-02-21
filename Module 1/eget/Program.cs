using System;
using Mono.Options;

namespace Eget
{
    public class MainClass
    {
        private enum CmdArgsParseStatus
        {
            Ok,
            ShowedHelp,
            ShowedError
        }

        public static int Main(string[] args)
        {
            string inputUrl;
            CrawlerOptions crawlerOptions;

            var cmdArgsStats = ParseCommandLineArgs(args, out inputUrl, out crawlerOptions);
            switch (cmdArgsStats)
            {
                case CmdArgsParseStatus.ShowedError:
                    return 1;
                case CmdArgsParseStatus.ShowedHelp:
                    return 0;
                case CmdArgsParseStatus.Ok:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var crawler = new Crawler(crawlerOptions);
            crawler.DownloadPage(new Uri(inputUrl));
            return 0;
        }

        private static CmdArgsParseStatus ParseCommandLineArgs(string[] args, out string inputUrl, out CrawlerOptions crawlerOptions)
        {
            int? maxDepth = null;
            var followPageLinks = false;
            var followResourceLinks = false;
            var uriRestrictions = UriFollowRestrictions.NoRestrictions;
            string outputDir;
            inputUrl = null;
            var enableLogging = false;
            var showHelp = false;

            var options = new OptionSet();
            options.Add("d|depth=", "maximum depth of followed links", (int depth) =>
            {
                if (depth < 0)
                    throw new OptionException("`--depth` must be a non-negative integer", "depth");
                maxDepth = depth;
            });
            options.Add("fp|follow-pages", "download and replace links to other pages", x => followPageLinks = x != null);
            options.Add("fr|follow-resources", "download and replace links to various resources(css, javascript, images)",
                x => followResourceLinks = x != null);
            options.Add("ru|restrict-urls=", "restrict followed links(`none`, `suburl`, `host`)", x =>
            {
                if (x == "none")
                    uriRestrictions = UriFollowRestrictions.NoRestrictions;
                else if (x == "suburl")
                    uriRestrictions = UriFollowRestrictions.FromSuburl;
                else if (x == "host")
                    uriRestrictions = UriFollowRestrictions.FromSameHost;
                else
                    throw new OptionException("invalid value for `--restrict-urls`", "restrict-urls");
            });
            options.Add("v|verbose", "verbose output", x => enableLogging = x != null);
            options.Add("h|help", "show help", h => showHelp = h != null);

            try
            {
                var extra = options.Parse(args);
                if (showHelp)
                {
                    Console.WriteLine("Usage: eget.exe [option1 option2 ...] <input-url> <output-dir>");
                    Console.WriteLine();
                    Console.WriteLine("Options:");
                    options.WriteOptionDescriptions(Console.Out);
                    crawlerOptions = null;
                    return CmdArgsParseStatus.ShowedHelp;
                }

                if (extra.Count != 2)
                {
                    PrintErrorMessage("exactly two arguments must be provided: <input-url> and <output-dir>");
                    crawlerOptions = null;
                    return CmdArgsParseStatus.ShowedError;
                }

                inputUrl = extra[0];
                outputDir = extra[1];
            }
            catch (OptionException e)
            {
                PrintErrorMessage(e.Message);
                crawlerOptions = null;
                return CmdArgsParseStatus.ShowedError;
            }

            crawlerOptions = new CrawlerOptions(maxDepth, outputDir, followPageLinks, followResourceLinks, uriRestrictions, enableLogging);
            return CmdArgsParseStatus.Ok;
        }

        private static void PrintErrorMessage(string errorMessage)
        {
            Console.Write("eget: ");
            Console.WriteLine(errorMessage);
            Console.WriteLine("Try `eget --help' for more information.");
        }
    }
}
