using System;
using UnityEngine;

namespace client_module_network
{
    public class NetMsgBase
    {    //协议名    
        public string protoName = "";

        //编码    
        public static byte[] Encode(NetMsgBase msgBase)
        {
            string s = JsonUtility.ToJson(msgBase);
            return System.Text.Encoding.UTF8.GetBytes(s);
        }

        //解码    
        public static NetMsgBase Decode(string protoName,byte[] bytes, int offset, int count)
        {
            string s = System.Text.Encoding.UTF8.GetString(bytes, offset, count);
            NetMsgBase msgBase = (NetMsgBase)JsonUtility.FromJson(s,
                Type.GetType(protoName));
            return msgBase;
        }

        //编码协议名(2字节长度+字符串)
        public static byte[] EncodeName(NetMsgBase msgBase)
        {
            //名字bytes和长度    
            byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(msgBase.protoName);
            short len = (short)nameBytes.Length;
            //申请bytes数值    
            byte[] bytes = new byte[2+len];
            //组装2字节的长度信息    
            bytes[0] = (byte)(len%256);
            bytes[1] = (byte)(len/256);
            //组装名字bytes
            Array.Copy(nameBytes, 0, bytes, 2, len);
            return bytes;
        }

        //解码协议名(2字节长度+字符串)
        public static string DecodeName(byte[] bytes, int offset, out int count)
        {
            count = 0;
            //必须大于2字节    
            if (offset + 2 > bytes.Length)
            {
                return "";
            }
            //读取长度    
            short len = (short)((bytes[offset+1] << 8 )| bytes[offset] );
            //长度必须足够    
            if (offset + 2 + len > bytes.Length)
            {
                return "";
            }
            //解析    
            count = 2+len;
            string name = System.Text.Encoding.UTF8.GetString(bytes, offset + 2, len);
            return name;
        }
    }

    public class MsgMove : NetMsgBase
    {
        public MsgMove()
        {
            protoName = "MsgMove";
        }
        public int x = 0; public int y = 0; public int z = 0;
    }

    public class MsgPing : NetMsgBase
    {
        public MsgPing()
        {
            protoName = "MsgPing";
        }
    }

    public class MsgPong : NetMsgBase
    {
        public MsgPong()
        {
            protoName = "MsgPong";
        }
    }

    //同步协议
    public class MsgFrameSync: NetMsgBase
    {
        public MsgFrameSync()
        {
            protoName = "MsgFrameSync";
        }
        //指令，0-前进 1-后退 2-左转 3-右转  4-停止  ...    
        public int cmd = 0;
        //在第几帧发生事件    
        public int frame = 0;
    }
}



