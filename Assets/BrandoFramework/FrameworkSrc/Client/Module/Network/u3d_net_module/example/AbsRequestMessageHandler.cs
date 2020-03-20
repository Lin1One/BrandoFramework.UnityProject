
namespace client_module_network
{
    public abstract class AbsRequestMessageHandler:  IRequestMessageHandler
    {
        public abstract int MessageId { get; }
        public abstract byte[] GetMessage(object obj);
    }
}

