

namespace client_module_network
{
    /// <summary>
    /// 消息答复处理器。
    /// </summary>
    public interface IRespMessageHandler : IMessageHandler
    {
        void ReceiveMessage(byte[] message);
    }
}

