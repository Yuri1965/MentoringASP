using System.IO;
using System.Runtime.Serialization;
using System.Web;

namespace MVCPeopleAwards.Helpers
{
    [DataContractAttribute]
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

        [DataMemberAttribute]
        public override int ContentLength => fileBytes.Length;

        [DataMemberAttribute]
        public override string FileName { get; }

        [DataMemberAttribute]
        public override Stream InputStream { get; }
    }
}