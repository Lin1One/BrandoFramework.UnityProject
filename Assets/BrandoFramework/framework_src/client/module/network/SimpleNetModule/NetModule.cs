using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;
namespace client_module_network
{
    public static class NetModule
    {
        static Socket socket;

        //���ջ�����
        static ByteArray readBuff;

        static int buffCount = 0;

        //д�����
        static Queue<ByteArray> writeQueue = new Queue<ByteArray>();

        static List<NetMsgBase> msgList = new List<NetMsgBase>();
        
        //��Ϣ�б���    
        static int msgCount = 0;

        //ÿһ��Update�������Ϣ��    
        readonly static int MAX_MESSAGE_FIRE = 10;

        //�Ƿ���������
        public static bool isUsePing = true;
        //�������ʱ��
        public static int pingInterval = 30;
        //��һ�η���PING��ʱ��
        static float lastPingTime = 0;
        //��һ���յ�PONG��ʱ��
        static float lastPongTime = 0; 

        /// <summary>
        /// ��ʼ��״̬
        /// </summary>
        private static void InitState()
        {
            //Socket    
            socket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            //���ջ�����
            readBuff = new ByteArray();
            //д�����
            writeQueue = new Queue<ByteArray>();
            //�Ƿ���������
            isConnecting = false;

            isClosing = false;

            //��Ϣ�б�    
            msgList = new List<NetMsgBase>();
            //��Ϣ�б���    
            msgCount = 0;
            //��һ�η���PING��ʱ��
            lastPingTime = Time.time;
            //��һ���յ�PONG��ʱ��
            lastPongTime = Time.time;

            //����PONGЭ��    
            if (!msgListeners.ContainsKey("MsgPong"))
            {
                AddMsgListener("MsgPong", OnMsgPong);
            }
        }



        #region ��Ϣ

        //��Ϣί������
        public delegate void MsgListener(NetMsgBase msgBase);
        //��Ϣ�����б�
        private static Dictionary<string, MsgListener> msgListeners
            = new Dictionary<string, MsgListener>();
        //�����Ϣ����
        public static void AddMsgListener(string msgName, MsgListener listener)
        {
            //���    
            if (msgListeners.ContainsKey(msgName))
            {
                msgListeners[msgName] += listener;
            }
            //����    
            else
            {
                msgListeners[msgName] = listener;
            }
        }

        //ɾ����Ϣ����
        public static void RemoveMsgListener(string msgName, MsgListener listener)
        {
            if (msgListeners.ContainsKey(msgName))
            {
                msgListeners[msgName] -= listener;
            }
            //ɾ��        
            if (msgListeners[msgName] == null)
            {
                msgListeners.Remove(msgName);
            }
        }

        //�ַ���Ϣ
        private static void FireMsg(string msgName, NetMsgBase msgBase)
        {
            if (msgListeners.ContainsKey(msgName))
            {
                msgListeners[msgName](msgBase);
            }
        }

        #endregion

        #region �¼�

        //�¼�ί������
        public delegate void EventListener(string err);
        //�¼������б�
        private static Dictionary<NetEvent, EventListener>
            eventListeners = new Dictionary<NetEvent, EventListener>();

        //����¼�����
        public static void AddEventListener(NetEvent netEvent,EventListener listener)
        {
            //����¼�
            if (eventListeners.ContainsKey(netEvent))
            {
                eventListeners[netEvent] += listener;
            }
            //�����¼�    
            else
            {
                eventListeners[netEvent] = listener;
            }
        }
        //ɾ���¼�����
        public static void RemoveEventListener(NetEvent netEvent, EventListener listener)
        {
            if (eventListeners.ContainsKey(netEvent))
            {
                eventListeners[netEvent] -= listener;
                //ɾ��
                if (eventListeners[netEvent] == null)
                {
                    eventListeners.Remove(netEvent);
                }
            }
        }

        private static void FireEvent(NetEvent netEvent, string err)
        {
            if (eventListeners.ContainsKey(netEvent))
            {
                eventListeners[netEvent](err);
            }
        }

        #endregion

        #region ����

        //�Ƿ���������
        static bool isConnecting = false; 

        //����    
        public static void Connect(string ip, int port)
        {
            if(socket != null && socket.Connected)
            {
                Debug.Log("Connect fail, already connected!");
                return;
            }
            //��ʼ����Ա
            InitState();
            //��������
            socket.NoDelay = true;
            //Connect
            isConnecting = true;
            socket.BeginConnect(ip, port, ConnectCallback, socket);

            //ͬ������
            //Socket        
            //socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Connect ����ͬ����ʽ�򻯴��룩
            //socket.Connect(ip, port);
            //BeginReceive
            //socket.BeginReceive(readBuff, buffCount, 1024 - buffCount, 0, ReceiveCallback, socket);
        }

        /// <summary>
        /// Connect�ص�
        /// </summary>
        /// <param name="ar"></param>
        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket) ar.AsyncState;
                socket.EndConnect(ar);
                Debug.Log("Socket Connect Succ ");
                FireEvent(NetEvent.ConnectSucc, "");
                isConnecting = false;
                //��ʼ����        
                socket.BeginReceive( 
                    readBuff.bytes, readBuff.writeIdx,
                    readBuff.remain, 0, ReceiveCallback, socket);
            }
            catch (SocketException ex)
            {
                Debug.Log("Socket Connect fail " + ex.ToString());
                FireEvent(NetEvent.ConnectFail, ex.ToString());
                isConnecting = false;
            }
        }

        #endregion

        #region �ر�

        //�Ƿ����ڹر�
        static bool isClosing = false;

        //�ر�����
        public static void Close()
        {
            //״̬�ж�    
            if (socket == null || !socket.Connected)
            {
                return;
            }
            if (isConnecting)
            {
                return;
            }
            //���������ڷ���    
            if (writeQueue.Count > 0)
            {
                isClosing = true;
            }
            //û�������ڷ���
            else
            {
                socket.Close();
                FireEvent(NetEvent.Close, "");
            }
        }

        #endregion

        #region ����

        //��������
        public static void Send(NetMsgBase msg)
        {
            //״̬�ж�
            if (socket == null || !socket.Connected)
            {
                return;
            }
            if (isConnecting)
            {
                return;
            }
            if (isClosing)
            {
                return;
            }
            //���ݱ���
            byte[] nameBytes = NetMsgBase.EncodeName(msg);
            byte[] bodyBytes = NetMsgBase.Encode(msg);
            int len = nameBytes.Length + bodyBytes.Length;

            byte[] sendBytes = new byte[2 + len];
            //��װ����        
            sendBytes[0] = (byte)(len % 256);
            sendBytes[1] = (byte)(len / 256);

            //��װ����        
            Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);
            //��װ��Ϣ��        
            Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);

            //д�����        
            ByteArray ba = new ByteArray(sendBytes);
            int count = 0;
            //writeQueue�ĳ���
            lock (writeQueue)
            {
                writeQueue.Enqueue(ba);
                count = writeQueue.Count;
            }
            if (count == 1)
            {
                socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);
            }
        }

        //Send �ص�
        public static void SendCallback(IAsyncResult ar)
        {
            //��ȡstate��EndSend�Ĵ���
            Socket socket = (Socket) ar.AsyncState;
            //״̬�ж�    
            if (socket == null || !socket.Connected)
            {
                return;
            }
            //EndSend    
            int count = socket.EndSend(ar);
            //��ȡд����е�һ������            
            ByteArray ba;
            lock (writeQueue)
            {
                ba = writeQueue.First();
            }
            //��������
            ba.readIdx += count;
            if (ba.length == 0)
            {
                lock (writeQueue)
                {
                    writeQueue.Dequeue();
                    ba = writeQueue.First();
                }
            }
            //��������    
            if (ba != null)
            {
                socket.BeginSend(ba.bytes, ba.readIdx, ba.length,
                    0, SendCallback, socket);
            }
            //���ڹر�    
            else if(isClosing)
            {
                socket.Close();
            }
        }

        #endregion

        #region ����

        //Receive�ص�
        public static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket) ar.AsyncState;
                //��ȡ�������ݳ���
                int count = socket.EndReceive(ar);
                if (count == 0)
                {
                    Close();
                    return;
                }
                readBuff.writeIdx+=count;
                //�����������Ϣ
                OnReceiveData();
                //������������
                if (readBuff.remain < 8)
                {
                    readBuff.MoveBytes();
                    readBuff.ReSize(readBuff.length * 2);
                }
                socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCallback, socket);
            }
            catch (SocketException ex)
            {
                Debug.Log("Socket Receive fail" + ex.ToString());
            }
        }

        private static void OnReceiveData()
        {
            //��Ϣ����
            if (buffCount <= 2)
            {
                return;
            }

            byte[] bytes = readBuff.bytes;

            //��ȡ��Ϣ�峤��        
            int readIdx = readBuff.readIdx;
            short bodyLength = (short)((bytes[readIdx + 1] << 8) | bytes[readIdx]);

            if (readBuff.length < bodyLength)
            {
                return;
            }

            readBuff.readIdx += 2;
            //����Э����        
            int nameCount = 0;

            string protoName = NetMsgBase.DecodeName(readBuff.bytes, readBuff.readIdx, out nameCount);

            if (protoName == "")
            {
                Debug.Log("OnReceiveData MsgBase.DecodeName fail");
                return;
            }
            readBuff.readIdx += nameCount;

            //����Э����        
            int bodyCount = bodyLength - nameCount;
            NetMsgBase msgBase = NetMsgBase.Decode(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);
            readBuff.readIdx += bodyCount;
            readBuff.CheckAndMoveBytes();
            //��ӵ���Ϣ����
            lock (msgList)
            {
                msgList.Add(msgBase);
            }
            msgCount++;
            //������ȡ��Ϣ
            if (readBuff.length > 2)
            {
                OnReceiveData();
            }
        }

        #endregion

        #region ֡ѭ��

        //Update    
        public static void Update()
        { 
            MsgUpdate();
            PingUpdate();
        }

        //������Ϣ
        public static void MsgUpdate()
        {
            //�����жϣ�����Ч��
            if (msgCount == 0)
            {
                return;
            }
            //�ظ�������Ϣ
            for (int i = 0; i< MAX_MESSAGE_FIRE; i++)
            {
                //��ȡ��һ����Ϣ 
                NetMsgBase msgBase = null;

                lock (msgList)
                {
                    if (msgList.Count > 0)
                    {
                        msgBase = msgList[0];
                        msgList.RemoveAt(0);
                        msgCount--;
                    }
                }

                //�ַ���Ϣ
                if (msgBase != null)
                {
                    FireMsg(msgBase.protoName, msgBase);
                }
                //û����Ϣ��
                else {
                    break;
                }
            }
        }

        //����PINGЭ��
        private static void PingUpdate()
        {
            //�Ƿ�����
            if (!isUsePing)
            {
                return;
            }
            //����PING
            if (Time.time - lastPingTime > pingInterval)
            {
                MsgPing msgPing = new MsgPing(); Send(msgPing); lastPingTime = Time.time;
            }
            //���PONGʱ��
            if (Time.time - lastPongTime > pingInterval * 4)
            {
                Close();
            }
        }

        //����PONGЭ��
        private static void OnMsgPong(NetMsgBase msgBase)
        {
            lastPongTime = Time.time;
        }
        #endregion

        #region ��С�˴���
        //public static short ToInt16(byte[] value, int startIndex)
        //{
        //    if (startIndex % 2 == 0)
        //    { // data is aligned
        //        return *((short*)pbyte);
        //    }
        //    else
        //    {
        //        if (IsLittleEndian)
        //        {
        //            return (short)((*pbyte) | (*(pbyte + 1) << 8));
        //        }
        //        else
        //        {
        //            return (short)((*pbyte << 8) | (*(pbyte + 1)));

        //        }
        //    }
        //}


    }
    #endregion
}


