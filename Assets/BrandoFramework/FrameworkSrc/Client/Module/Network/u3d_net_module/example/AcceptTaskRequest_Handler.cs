

namespace client_module_network
{
    public class AcceptTaskRequest_Handler : AbsRequestMessageHandler
    {
        public override int MessageId => 18001;
        public override byte[] GetMessage(object obj)
        {
            int taskType = (int)obj;
            string msg = "RequestMessage";

            return NetCodeUtility.GetSendBytes(msg, 18001);
        }
    }
}

