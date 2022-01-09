using System.ServiceModel;

namespace WCFChat
{
    public class ServerUser
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public OperationContext operationContext { get; set; }
    }

    public class ServerFiles
    {
        public byte[] FileBytes { get; set; }
    }
}
