using System.IO;
using System.Web;

namespace MVCPeopleAwards.Helpers
{
    public class ExtHttpPostedFileBase : HttpPostedFileBase
    {
        private readonly byte[] fileBytes;

        public ExtHttpPostedFileBase(byte[] fileBytes, string fileName = null)
        {
            this.fileBytes = fileBytes;
            this.FileName = fileName;

            if (fileBytes != null && fileBytes.Length > 0)
                this.InputStream = new MemoryStream(fileBytes);
            else
                this.InputStream = null;
        }

        public override int ContentLength => fileBytes.Length;

        public override string FileName { get; }

        public override Stream InputStream { get; }
    }
}