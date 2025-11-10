namespace TDDLab.Core.Infrastructure
{
    public class Message<DataType>
    {
        public Metadata Metadata { get; set; }
        public DataType Data { get; set; }
    }
}