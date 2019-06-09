#region Head

// Author:            Yu
// CreateDate:        2018/7/25 6:23:47
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using client_common;
using System;


namespace client_module_network
{
    /// <summary>
    /// 通信下行解码器。
    /// </summary>
    public class MessageDeCodeHandler : IMessageDownHandler
    {
        /// <summary>
        /// 协议头
        /// </summary>
        private byte[] headBytes = new byte[4];

        /// <summary>
        /// 消息头长度
        /// </summary>
        private int messageHeadLength;

        /// <summary>
        /// 是否大端
        /// </summary>
        private bool _isBigPoint;

        public byte[] Handle(IByteBuf byteBuf)
        {
            HeadLength();
            SetIsBigPoint(true);

            //内容少于表示长度的字节数
            if (byteBuf.ReadbleCount < messageHeadLength)
            {
                return null;
            }

            headBytes = TryConvertBigPoint(byteBuf);

            // 前四个字节为消息长度
            var messageLength = BitConverter.ToInt32(headBytes, 0);

            //不足长度
            if (messageLength > byteBuf.ReadbleCount)
            {
                byteBuf.MoveReadIndex(-messageHeadLength);
                return null;
            }

            var messageBytes = byteBuf.ReadBytes(messageLength);
            byteBuf.LastBytes = messageBytes;
            return messageBytes;
        }

        private byte[] TryConvertBigPoint(IByteBuf byteBuf)
        {
            var bytes = byteBuf.ReadBytes(messageHeadLength);
            if (!_isBigPoint)
            {
                return bytes;
            }
            bytes = ToBig(bytes);
            return bytes;
        }

        public void HeadLength(int length = 4)
        {
            messageHeadLength = length;
        }

        public void SetIsBigPoint(bool isBigPoint)
        {
            _isBigPoint = isBigPoint;
        }

        /// <summary>
        /// 解析服务器答复消息的消息长度
        /// 大小端转换
        /// </summary>
        private byte[] ToBig(byte[] bytes)
        {
            var targetBytes = new byte[bytes.Length];
            if(_isBigPoint)
            {
                for (var i = 3; i >= 0; i--)
                {
                    targetBytes[i] = bytes[3 - i];
                }
            }
            else
            {
                targetBytes = bytes;
            }
            return targetBytes;
        }
    }
}