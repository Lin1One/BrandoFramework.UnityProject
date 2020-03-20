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

        //接收缓冲区
        static ByteArray readBuff;

        static int buffCount = 0;

        //写入队列
        static Queue<ByteArray> writeQueue = new Queue<ByteArray>();

        static List<NetMsgBase> msgList = new List<NetMsgBase>();
        
        //消息列表长度    
        static int msgCount = 0;

        //每一次Update处理的消息量    
        readonly static int MAX_MESSAGE_FIRE = 10;

        //是否启用心跳
        public static bool isUsePing = true;
        //心跳间隔时间
        public static int pingInterval = 30;
        //上一次发送PING的时间
        static float lastPingTime = 0;
        //上一次收到PONG的时间
        static float lastPongTime = 0; 

        /// <summary>
        /// 初始化状态
        /// </summary>
        private static void InitState()
        {
            //Socket    
            socket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            //接收缓冲区
            readBuff = new ByteArray();
            //写入队列
            writeQueue = new Queue<ByteArray>();
            //是否正在连接
            isConnecting = false;

            isClosing = false;

            //消息列表    
            msgList = new List<NetMsgBase>();
            //消息列表长度    
            msgCount = 0;
            //上一次发送PING的时间
            lastPingTime = Time.time;
            //上一次收到PONG的时间
            lastPongTime = Time.time;

            //监听PONG协议    
            if (!msgListeners.ContainsKey("MsgPong"))
            {
                AddMsgListener("MsgPong", OnMsgPong);
            }
        }



        #region 消息

        //消息委托类型
        public delegate void MsgListener(NetMsgBase msgBase);
        //消息监听列表
        private static Dictionary<string, MsgListener> msgListeners
            = new Dictionary<string, MsgListener>();
        //添加消息监听
        public static void AddMsgListener(string msgName, MsgListener listener)
        {
            //添加    
            if (msgListeners.ContainsKey(msgName))
            {
                msgListeners[msgName] += listener;
            }
            //新增    
            else
            {
                msgListeners[msgName] = listener;
            }
        }

        //删除消息监听
        public static void RemoveMsgListener(string msgName, MsgListener listener)
        {
            if (msgListeners.ContainsKey(msgName))
            {
                msgListeners[msgName] -= listener;
            }
            //删除        
            if (msgListeners[msgName] == null)
            {
                msgListeners.Remove(msgName);
            }
        }

        //分发消息
        private static void FireMsg(string msgName, NetMsgBase msgBase)
        {
            if (msgListeners.ContainsKey(msgName))
            {
                msgListeners[msgName](msgBase);
            }
        }

        #endregion

        #region 事件

        //事件委托类型
        public delegate void EventListener(string err);
        //事件监听列表
        private static Dictionary<NetEvent, EventListener>
            eventListeners = new Dictionary<NetEvent, EventListener>();

        //添加事件监听
        public static void AddEventListener(NetEvent netEvent,EventListener listener)
        {
            //添加事件
            if (eventListeners.ContainsKey(netEvent))
            {
                eventListeners[netEvent] += listener;
            }
            //新增事件    
            else
            {
                eventListeners[netEvent] = listener;
            }
        }
        //删除事件监听
        public static void RemoveEventListener(NetEvent netEvent, EventListener listener)
        {
            if (eventListeners.ContainsKey(netEvent))
            {
                eventListeners[netEvent] -= listener;
                //删除
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

        #region 连接

        //是否正在连接
        static bool isConnecting = false; 

        //连接    
        public static void Connect(string ip, int port)
        {
            if(socket != null && socket.Connected)
            {
                Debug.Log("Connect fail, already connected!");
                return;
            }
            //初始化成员
            InitState();
            //参数设置
            socket.NoDelay = true;
            //Connect
            isConnecting = true;
            socket.BeginConnect(ip, port, ConnectCallback, socket);

            //同步代码
            //Socket        
            //socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Connect （用同步方式简化代码）
            //socket.Connect(ip, port);
            //BeginReceive
            //socket.BeginReceive(readBuff, buffCount, 1024 - buffCount, 0, ReceiveCallback, socket);
        }

        /// <summary>
        /// Connect回调
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
                //开始接收        
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

        #region 关闭

        //是否正在关闭
        static bool isClosing = false;

        //关闭连接
        public static void Close()
        {
            //状态判断    
            if (socket == null || !socket.Connected)
            {
                return;
            }
            if (isConnecting)
            {
                return;
            }
            //还有数据在发送    
            if (writeQueue.Count > 0)
            {
                isClosing = true;
            }
            //没有数据在发送
            else
            {
                socket.Close();
                FireEvent(NetEvent.Close, "");
            }
        }

        #endregion

        #region 发送

        //发送数据
        public static void Send(NetMsgBase msg)
        {
            //状态判断
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
            //数据编码
            byte[] nameBytes = NetMsgBase.EncodeName(msg);
            byte[] bodyBytes = NetMsgBase.Encode(msg);
            int len = nameBytes.Length + bodyBytes.Length;

            byte[] sendBytes = new byte[2 + len];
            //组装长度        
            sendBytes[0] = (byte)(len % 256);
            sendBytes[1] = (byte)(len / 256);

            //组装名字        
            Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);
            //组装消息体        
            Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);

            //写入队列        
            ByteArray ba = new ByteArray(sendBytes);
            int count = 0;
            //writeQueue的长度
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

        //Send 回调
        public static void SendCallback(IAsyncResult ar)
        {
            //获取state、EndSend的处理
            Socket socket = (Socket) ar.AsyncState;
            //状态判断    
            if (socket == null || !socket.Connected)
            {
                return;
            }
            //EndSend    
            int count = socket.EndSend(ar);
            //获取写入队列第一条数据            
            ByteArray ba;
            lock (writeQueue)
            {
                ba = writeQueue.First();
            }
            //完整发送
            ba.readIdx += count;
            if (ba.length == 0)
            {
                lock (writeQueue)
                {
                    writeQueue.Dequeue();
                    ba = writeQueue.First();
                }
            }
            //继续发送    
            if (ba != null)
            {
                socket.BeginSend(ba.bytes, ba.readIdx, ba.length,
                    0, SendCallback, socket);
            }
            //正在关闭    
            else if(isClosing)
            {
                socket.Close();
            }
        }

        #endregion

        #region 接收

        //Receive回调
        public static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket) ar.AsyncState;
                //获取接收数据长度
                int count = socket.EndReceive(ar);
                if (count == 0)
                {
                    Close();
                    return;
                }
                readBuff.writeIdx+=count;
                //处理二进制消息
                OnReceiveData();
                //继续接收数据
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
            //消息长度
            if (buffCount <= 2)
            {
                return;
            }

            byte[] bytes = readBuff.bytes;

            //获取消息体长度        
            int readIdx = readBuff.readIdx;
            short bodyLength = (short)((bytes[readIdx + 1] << 8) | bytes[readIdx]);

            if (readBuff.length < bodyLength)
            {
                return;
            }

            readBuff.readIdx += 2;
            //解析协议名        
            int nameCount = 0;

            string protoName = NetMsgBase.DecodeName(readBuff.bytes, readBuff.readIdx, out nameCount);

            if (protoName == "")
            {
                Debug.Log("OnReceiveData MsgBase.DecodeName fail");
                return;
            }
            readBuff.readIdx += nameCount;

            //解析协议体        
            int bodyCount = bodyLength - nameCount;
            NetMsgBase msgBase = NetMsgBase.Decode(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);
            readBuff.readIdx += bodyCount;
            readBuff.CheckAndMoveBytes();
            //添加到消息队列
            lock (msgList)
            {
                msgList.Add(msgBase);
            }
            msgCount++;
            //继续读取消息
            if (readBuff.length > 2)
            {
                OnReceiveData();
            }
        }

        #endregion

        #region 帧循环

        //Update    
        public static void Update()
        { 
            MsgUpdate();
            PingUpdate();
        }

        //更新消息
        public static void MsgUpdate()
        {
            //初步判断，提升效率
            if (msgCount == 0)
            {
                return;
            }
            //重复处理消息
            for (int i = 0; i< MAX_MESSAGE_FIRE; i++)
            {
                //获取第一条消息 
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

                //分发消息
                if (msgBase != null)
                {
                    FireMsg(msgBase.protoName, msgBase);
                }
                //没有消息了
                else {
                    break;
                }
            }
        }

        //发送PING协议
        private static void PingUpdate()
        {
            //是否启用
            if (!isUsePing)
            {
                return;
            }
            //发送PING
            if (Time.time - lastPingTime > pingInterval)
            {
                MsgPing msgPing = new MsgPing(); Send(msgPing); lastPingTime = Time.time;
            }
            //检测PONG时间
            if (Time.time - lastPongTime > pingInterval * 4)
            {
                Close();
            }
        }

        //监听PONG协议
        private static void OnMsgPong(NetMsgBase msgBase)
        {
            lastPongTime = Time.time;
        }
        #endregion

        #region 大小端处理
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


