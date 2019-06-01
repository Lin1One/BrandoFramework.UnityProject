#region Head

// Author:            Yu
// CreateDate:        2018/10/28 17:33:07
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using client_common;
using Common.DataStruct;
using System;

namespace client_module_network
{
    public static class NetCodeUtility
    {
        private static readonly ISerializer serializer;

        static NetCodeUtility()
        {
            serializer = new ProtobufSerializer();
        }

        /// <summary>
        /// 以前 4 个字节作为头部协议号
        /// </summary>
        /// <param name="message"></param>
        /// <param name="msgId"></param>
        /// <returns></returns>
        public static byte[] GetSendBytes(object message, int msgId)
        {
            byte[] messageBytes = serializer.Serialize(message);
            var tempBytes = BitConverter.GetBytes(msgId);
            byte[] msgIdBytes = tempBytes.GetHeadBytes();
            byte[] finalBytes = BytesPool.Take(4 + messageBytes.Length);
            for (int i = 0; i < 4; i++)
            {
                finalBytes[i] = msgIdBytes[i];
            }

            for (int i = 0; i < messageBytes.Length; i++)
            {
                finalBytes[i + 4] = messageBytes[i];
            }

            return finalBytes;
        }
    }
}