using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Text.Json;
using Proto;

namespace ConsoleApplicatLocalServer
{

    struct PlayerInfo
    {
        public int guid;
        public float lastHeartTime;
        public TcpClient client;
        public NetworkStream stream;
    }
    
    public class ServerLogic
    {
        private static int guid = 0;
        private string ip = "192.168.50.22";
        private int pot = 8080;
        IPEndPoint ipPoint;
        private TcpListener tcpListener;
        private List<PlayerInfo> _playerInfos = new List<PlayerInfo>();
        byte[] streamBuffer = new byte[1024 * 1024];
        private BinaryFormatter _serializer = new BinaryFormatter(); 
        
        public void StartServer()
        {
            // Console.Write("InputIP\n");
            // ip = Console.ReadLine();
            // Console.Write($"19 ip:{ip} pot:{pot}");
            IPAddress ipAdress = IPAddress.Parse(ip);
            ipPoint = new IPEndPoint(ipAdress, pot);
            
            tcpListener = new TcpListener(ipPoint);
            Thread collect = new Thread(Collecting);
            collect.Start();
        }

        public void Update(int deltTime)
        {
            RecievePlayerInfo();
        }

        public void Collecting()
        {
            tcpListener.Start();
            while (true)
            {
                var tcpClient = tcpListener.AcceptTcpClient();
                
                var newPlayer = new PlayerInfo()
                {
                    guid = guid++ ,
                    client = tcpClient,
                    stream = tcpClient.GetStream()
                };
                var logInfo = new PlayerLogInfo() { guid = newPlayer.guid };
                string jsn = JsonSerializer.Serialize(logInfo);
                byte[] data = Encoding.UTF8.GetBytes(jsn);
                newPlayer.stream.Write(data, 0, data.Length);
                
                
                
                lock (_playerInfos)
                {
                    _playerInfos.Add(newPlayer);
                }
            }
        }

        public void RecievePlayerInfo()
        {
            lock (_playerInfos)
            {
                for (int i = 0; i < _playerInfos.Count; i++)
                {
                    var playerInfo = _playerInfos[i];
                    var stream = playerInfo.stream;
                    var readLen = stream.Read(streamBuffer, 0, streamBuffer.Length);
                    if (readLen > 0)
                    {
                        var msg = Encoding.UTF8.GetString(streamBuffer, 0, readLen);
                        Console.Write(msg);
                        string message = "recieved";
                        byte[] data = Encoding.UTF8.GetBytes(message);
                        playerInfo.client.Client.Send(data);
                    }
                }
            }
            
        }
    }
}