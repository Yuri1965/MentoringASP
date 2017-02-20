using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CsQuery;

using RipperManagerLib;

namespace Ripper
{
    class Program
    {
        private static string outputDir = @"d:\Temp\Wikipedia\";
        private static string siteLink = "https://en.wikipedia.org";

        static void Main(string[] args)
        {

            Console.WriteLine("Press any key for starting...");
            Console.ReadKey();

            RipperManager ripM = new RipperManager(siteLink, outputDir, DownloadOptionDomain.Domain, DownloadOptionLevel.Level1);

            ripM.SaveSite();

            Console.ReadKey();
        }
    }
}
