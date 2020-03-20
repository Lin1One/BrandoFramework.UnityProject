
using Common;
using System;

namespace client_module_network
{
    public class MessageUpCodeHandler : IMessageUpHandler
    {
        public byte[] Handle(byte[] receiveBytes)
        {
            // todo 
            var bytes = BytesPool.Take(receiveBytes.Length + 4);
            //var bytes = new byte[receiveBytes.Length + 4];

            var lengthBytes = BitConverter.GetBytes(receiveBytes.Length);

            for (int i = 0; i < 4; i++)
            {
                bytes[i] = lengthBytes[3 - i];
            }

            for (int i = 0; i < receiveBytes.Length; i++)
            {
                bytes[i + 4] = receiveBytes[i];
            }

            return bytes;
        }
    }
}

