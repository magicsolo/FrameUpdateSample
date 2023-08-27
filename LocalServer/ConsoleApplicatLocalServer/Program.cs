using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ConsoleApplicatLocalServer
{
    internal class Program
    {
        public static void Main(string[] args)
        {

            Console.Write("ServerStart");
            
            Thread update = new Thread(Updating);
            update.Start();
            Console.Write("ServerEnd");
        }

        public static void Updating()
        {
            int secondTime = 100;
            var serverLogic = new ServerLogic();
            serverLogic.StartServer();
            serverLogic.Update(0);

            while (true)
            {
                serverLogic.Update(secondTime);
                Thread.Sleep(secondTime);
            }

            
        }
        //
        // private void InitServer()
        // {
        //     
        // }
        public void TestFunc()
        {
            // Console.WriteLine("udp server start");
            var socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEndPoint = new IPEndPoint(ip, 8080);
            socketServer.Bind(ipEndPoint);
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0); //任意一个UDP客户端都可以连接本服务器 无端口限制
            EndPoint senderRemote = (EndPoint)sender;
            byte[] data = new byte[1024 * 1024];
            while (true)
            {
                for (int i = 0; i < data.Length; i++)
                    data[i] = 0;

                int readLeng = socketServer.ReceiveFrom(data, data.Length, SocketFlags.None, ref senderRemote);
                var msg = Encoding.UTF8.GetString(data, 0, readLeng);
                Console.WriteLine(msg);
            }
        }
    }
}

//Socket通信
// Console.WriteLine("program Start");
// var socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

// socketServer.Bind(ipEndPoint);
// socketServer.Listen(10);
// Socket newSocket = socketServer.Accept();
// byte[] data = new byte[1024 * 1024];
// int readLeng = newSocket.Receive(data, 0, data.Length,SocketFlags.None);
// var msg = Encoding.UTF8.GetString(data, 0, readLeng);
// Console.WriteLine(msg);
// Console.ReadKey();
// Console.WriteLine("program End");

//TCP通信
// Console.WriteLine("服务器开启");
// var tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 9999);
// tcpListener.Start(10);
// var tcpClient = tcpListener.AcceptTcpClient();
// NetworkStream networkStream = tcpClient.GetStream();
// byte[] buffer = new byte[1024 * 1024];
// var readLen = networkStream.Read(buffer, 0, buffer.Length);
// var msg = Encoding.UTF8.GetString(buffer, 0, readLen);
// Console.WriteLine(msg);
// Console.ReadKey();
// Console.WriteLine("program End");

//UDP Socket通信
// Console.WriteLine("udp server start");
// var socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
// IPAddress ip = IPAddress.Parse("127.0.0.1");
// IPEndPoint ipEndPoint = new IPEndPoint(ip, 8080);
// socketServer.Bind(ipEndPoint);
// IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);//任意一个UDP客户端都可以连接本服务器 无端口限制
// EndPoint senderRemote = (EndPoint)sender;
// byte[] data = new byte[1024 * 1024];
// while (true)
// {
//     for (int i = 0; i < data.Length; i++)
//         data[i] = 0;
//     
//     int readLeng = socketServer.ReceiveFrom(data, data.Length, SocketFlags.None, ref senderRemote);
//     var msg = Encoding.UTF8.GetString(data, 0, readLeng);
//     Console.WriteLine(msg);
// }