using System.Text;

namespace TDDLab.Core.Infrastructure
{
    public class Metadata
    {
        byte[] metadataBlob;

        public Metadata FromString(string metadata)
        {
            metadataBlob = new UTF8Encoding().GetBytes(metadata);
            return this;
        }
    }
}