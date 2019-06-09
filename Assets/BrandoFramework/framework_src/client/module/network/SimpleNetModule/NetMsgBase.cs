using System;
using UnityEngine;

namespace client_module_network
{
    public class NetMsgBase
    {    //Э����    
        public string protoName = "";

        //����    
        public static byte[] Encode(NetMsgBase msgBase)
        {
            string s = JsonUtility.ToJson(msgBase);
            return System.Text.Encoding.UTF8.GetBytes(s);
        }

        //����    
        public static NetMsgBase Decode(string protoName,byte[] bytes, int offset, int count)
        {
            string s = System.Text.Encoding.UTF8.GetString(bytes, offset, count);
            NetMsgBase msgBase = (NetMsgBase)JsonUtility.FromJson(s,
                Type.GetType(protoName));
            return msgBase;
        }

        //����Э����(2�ֽڳ���+�ַ���)
        public static byte[] EncodeName(NetMsgBase msgBase)
        {
            //����bytes�ͳ���    
            byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(msgBase.protoName);
            short len = (short)nameBytes.Length;
            //����bytes��ֵ    
            byte[] bytes = new byte[2+len];
            //��װ2�ֽڵĳ�����Ϣ    
            bytes[0] = (byte)(len%256);
            bytes[1] = (byte)(len/256);
            //��װ����bytes
            Array.Copy(nameBytes, 0, bytes, 2, len);
            return bytes;
        }

        //����Э����(2�ֽڳ���+�ַ���)
        public static string DecodeName(byte[] bytes, int offset, out int count)
        {
            count = 0;
            //�������2�ֽ�    
            if (offset + 2 > bytes.Length)
            {
                return "";
            }
            //��ȡ����    
            short len = (short)((bytes[offset+1] << 8 )| bytes[offset] );
            //���ȱ����㹻    
            if (offset + 2 + len > bytes.Length)
            {
                return "";
            }
            //����    
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

    //ͬ��Э��
    public class MsgFrameSync: NetMsgBase
    {
        public MsgFrameSync()
        {
            protoName = "MsgFrameSync";
        }
        //ָ�0-ǰ�� 1-���� 2-��ת 3-��ת  4-ֹͣ  ...    
        public int cmd = 0;
        //�ڵڼ�֡�����¼�    
        public int frame = 0;
    }
}



