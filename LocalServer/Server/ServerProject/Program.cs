using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using C2SProtoInterface;
using Google.Protobuf;
using ThreadState = System.Threading.ThreadState;

namespace GameServer
{
    internal class Program
    {
        public static Thread update;

        public static void Main(string[] args)
        {

            string ip = null;
            if (args.Length>0)
                ip = args[0];
            StartServer(ip);
        }

        public static void StartServer(string ip)
        {
            Console.Write("ServerStart\n");
            serverLogic.StartServer(ip);
            update = new Thread(Updating);
            update.Start();

            // Thread receiveTcpDt = new Thread(ReceiveTCPData);
            // receiveTcpDt.Start();
            // while (true)
            // {
            //     var input = Console.ReadLine();
            //     if (input.Contains("-kill"))
            //     {
            //         Environment.Exit(0);
            //     }
            // }
            Console.Write("ServerEnd\n"); 
        }
        static ServerLogic serverLogic = new ServerLogic();

        // public static void ReceiveTCPData()
        // {
        //     while (true)
        //     {
        //         Thread.Sleep(0);
        //     }
        // }
        
        public static void Updating()
        {
            Console.Write("Updating Strat\n");
            while (true)
            {
                serverLogic.UpdateTCPInfo();
                serverLogic.Update();
                Thread.Sleep(ServerLogic.frameTime);
            }
        }

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