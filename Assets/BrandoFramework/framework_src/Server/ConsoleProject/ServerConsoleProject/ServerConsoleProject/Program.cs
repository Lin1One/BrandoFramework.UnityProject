
#if BrandoFramewordServer

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class TestServer
    {
        static Socket serverSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

        static Dictionary<Socket, ClientState> acceptSocket = new Dictionary<Socket, ClientState>();

        //同步收发
        //public static void Main(string[] args)
        //{


        //    IPAddress ip = IPAddress.Parse("127.0.0.1");
        //    IPEndPoint ipEp = new IPEndPoint(ip, 8888);
        //    serverSocket.Bind(ipEp);
        //    serverSocket.Listen(0);
        //    Console.WriteLine("Server Start Up");
        //    while (true)
        //    {
        //        Socket connfd = serverSocket.Accept();

        //        byte[] readBuff = new byte[1024];
        //        int count = connfd.Receive(readBuff);
        //        string readStr = System.Text.Encoding.Default.GetString(readBuff, 0, count);
        //        Console.WriteLine($"服务器接收到：{readStr}");
        //        byte[] sendBytes = System.Text.Encoding.Default.GetBytes("ServerSendBack");
        //        connfd.Send(sendBytes);
        //        //SendBack();
        //    }
        //}

        //异步监听
        public static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEp = new IPEndPoint(ip, 8888);
            serverSocket.Bind(ipEp);
            serverSocket.Listen(0);
            Console.WriteLine("Server Start Up");
            AsyncAceept();
            Console.ReadLine();
        }

        #region Poll 
        //Poll 收发
        public static void Main1(string[] args)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEp = new IPEndPoint(ip, 8888);
            serverSocket.Bind(ipEp);
            serverSocket.Listen(0);
            Console.WriteLine("Server Start Up");
            //主循环
            while (true)
            {
                //检查listenfd            
                if (serverSocket.Poll(0, SelectMode.SelectRead))
                {
                    ReadListenfd(serverSocket);
                }
                //检查clientfd            
                foreach (ClientState s in acceptSocket.Values)
                {
                    Socket clientfd = s.socket;
                    if (clientfd.Poll(0, SelectMode.SelectRead))
                    {
                        if (!ReadClientfd(clientfd))
                        {
                            break;
                        }
                    }
                }            //防止CPU占用过高
                System.Threading.Thread.Sleep(1);
            }

        }

        //读取Listenfd
        public static void ReadListenfd(Socket listenfd)
        {
            Console.WriteLine("Accept");
            Socket clientfd = listenfd.Accept();
            ClientState state = new ClientState(clientfd);
            state.socket = clientfd;
            acceptSocket.Add(clientfd, state);
        }

        //读取Clientfd
        public static bool ReadClientfd(Socket clientfd)
        {
            ClientState state = acceptSocket[clientfd];
            //接收
            int count = 0;
            try
            {
                count = clientfd.Receive(state.readBuff);
            }
            catch (SocketException ex)
            {
                clientfd.Close();
                acceptSocket.Remove(clientfd);
                Console.WriteLine("Receive SocketException " + ex.ToString());
                return false;
            }
            //客户端关闭    
            if (count == 0)
            {
                clientfd.Close();
                acceptSocket.Remove(clientfd);
                Console.WriteLine("Socket Close");
                return false;
            }
            //广播
            string recvStr =
                System.Text.Encoding.Default.GetString(state.readBuff, 0, count);
            Console.WriteLine("Receive" + recvStr);
            string sendStr = clientfd.RemoteEndPoint.ToString() + ":" + recvStr;
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
            foreach (ClientState cs in acceptSocket.Values)
            {
                cs.socket.Send(sendBytes);
            }
            return true;
        }


            #endregion

        public static void SendBack()
        {
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes("ServerSendBack");
            serverSocket.Send(sendBytes);
        }

        #region 监听

        public static void AsyncAceept()
        {
            serverSocket.BeginAccept(AcceptCallback, serverSocket);
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            var serverSocket = (Socket)ar.AsyncState;
            var clientSocket = serverSocket.EndAccept(ar);
            if (!acceptSocket.ContainsKey(clientSocket))
            {

                acceptSocket.Add(clientSocket, new ClientState(clientSocket));
            }
            ClientState client = acceptSocket[clientSocket];
            client.socket.BeginReceive(client.readBuff, 0, 1024,0, OnReceive,client.socket);
        }

        #endregion

        public static void OnReceive(IAsyncResult ar)
        {
            var socket = (Socket)ar.AsyncState;
            var count = socket.EndReceive(ar);
            if (count == 0)
            {
                socket.Close();
                acceptSocket.Remove(socket);
                Console.WriteLine("Socket Close");
                return;
            }

            var recvStr = System.Text.Encoding.Default.GetString(acceptSocket[socket].readBuff);
            //textControll.text = recvStr;
            Console.WriteLine($"收到消息：{recvStr}");
            socket.BeginReceive(acceptSocket[socket].readBuff, 0, 1024, 0, OnReceive, socket);


        }
    }

    class ClientState
    {
        public Socket socket;
        public byte[] readBuff = new byte[1024];

        public ClientState(Socket newSocket)
        {
            socket = newSocket;
        }
    }
}

#endif