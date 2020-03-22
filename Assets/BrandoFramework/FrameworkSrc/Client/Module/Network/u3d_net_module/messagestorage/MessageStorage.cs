#region Head

// Author:            Yu
// CreateDate:        2018/10/24 17:27:47
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using System.Collections.Generic;


namespace client_module_network
{
    /// <summary>
    /// 消息存储器。
    /// </summary>
    public class MessageStorage : IMessageStorage
    {
        public Queue<byte[]> ReceiveMessages { get; }
        public Queue<byte[]> SendMessages { get; }

        public int ReceiveCount => ReceiveMessages.Count;
        public int SendCount => SendMessages.Count;

        public MessageStorage()
        {
            ReceiveMessages = new Queue<byte[]>();
            SendMessages = new Queue<byte[]>();
        }

        public void PushSendMessage(byte[] sendBytes)
        {
            SendMessages.Enqueue(sendBytes);
        }

        public void PushReceiveMessage(byte[] receiveBytes)
        {
            if (receiveBytes == null)
            {
                return;
            }
            ReceiveMessages.Enqueue(receiveBytes);
        }

        public byte[] GetSendMessage()
        {
            var bytes = SendMessages.Dequeue();
            return bytes;
        }

        public byte[] GetReceiveMessage()
        {
            var bytes = ReceiveMessages.Dequeue();
            return bytes;
        }
    }
}