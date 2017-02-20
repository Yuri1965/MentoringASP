using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RipperManagerLib
{
    public static class UtilsLib
    {
        public static string GetFileNameFromUrl(string url)
        {
            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                Console.WriteLine("This reference (url) file name is not found");
                return "";
            }

            return Path.GetFileName(uri.LocalPath);
        }

        public static void SaveFile(string fileName, string content)
        {
            try
            {
                Console.WriteLine("Save file {0}", fileName);

                // создаем директорию для сохранения файла если ее нет
                DirectoryInfo dirInfo = new DirectoryInfo(Path.GetDirectoryName(fileName));
                if (!dirInfo.Exists)
                    dirInfo.Create();

                using (StreamWriter fileSave = new StreamWriter(fileName))
                {
                    fileSave.Write(content);
                    fileSave.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void SaveFile(string fileName, Stream content)
        {
            try
            {
                Console.WriteLine("Save file {0}", fileName);

                // создаем директорию для сохранения файла если ее нет
                DirectoryInfo dirInfo = new DirectoryInfo(Path.GetDirectoryName(fileName));
                if (!dirInfo.Exists)
                    dirInfo.Create();

                using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    content.CopyTo(fs);
                    fs.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
