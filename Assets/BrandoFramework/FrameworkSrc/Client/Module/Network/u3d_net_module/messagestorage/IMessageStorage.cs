
namespace client_module_network
{
    /// <summary>
    /// 消息存储器。
    /// 维护收发消息的队列，在帧循环中取用
    /// </summary>
    public interface IMessageStorage
    {
        void PushSendMessage(byte[] sendBytes);

        void PushReceiveMessage(byte[] receiveBytes);

        byte[] GetSendMessage();

        byte[] GetReceiveMessage();

        /// <summary>
        /// 当前可读的消息数量。
        /// </summary>
        int ReceiveCount { get; }

        /// <summary>
        /// 当前可发送的消息数量。
        /// </summary>
        int SendCount { get; }
    }
}