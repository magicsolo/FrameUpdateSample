using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using C2SProtoInterface;
using Google.Protobuf;

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
        private static int guid = 1;
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
            RecieveTCPInfo();
        }

        public void Collecting()
        {
            tcpListener.Start();
            while (true)
            {
                var tcpClient = tcpListener.AcceptTcpClient();

                var newPlayer = new PlayerInfo()
                {
                    guid = guid++,
                    client = tcpClient,
                    stream = tcpClient.GetStream()
                };
                lock (_playerInfos)
                {
                    _playerInfos.Add(newPlayer);
                }
            }
        }
        
        unsafe void SendTCPData(Stream stream,EMessage messageType,IMessage obj)
        {
            byte[] data = new byte[sizeof(EMessage) +sizeof(int) + obj.CalculateSize()]; //` Encoding.UTF8.GetBytes(jsn);
            var infoBytes = obj.ToByteArray();
            int offset = 0;
            fixed (byte* p = data)
            {
                *(EMessage*)p = messageType;
                offset += sizeof(EMessage);
                *(int*)(p + offset) = infoBytes.Length;
                offset += sizeof(int);
            }
            Buffer.BlockCopy(infoBytes, 0, data, offset, infoBytes.Length);
            stream.Write(data, 0, data.Length);
        }
        public void RecieveTCPInfo()
        {
            {
                lock (_playerInfos)
                {
                    for (int i = 0; i < _playerInfos.Count; i++)
                    {
                        OnRecieveTCPInfo(_playerInfos[i]);
                    }
                }
            
            }
        }

        unsafe void OnRecieveTCPInfo(PlayerInfo plInfo)
        {
            var stream = plInfo.stream;
            var readLen = stream.Read(streamBuffer, 0, streamBuffer.Length);
            if (readLen <= 0)
                return;
            EMessage msgType = default;
            fixed (void* p = streamBuffer)
                msgType = *(EMessage*)p;

            switch (msgType)
            {
                case EMessage.Login:
                    OnPlayerLogin(plInfo);
                    break;
            }
        }

        void OnPlayerLogin(PlayerInfo plInfo)
        {
            SendTCPData(plInfo.stream,EMessage.Login, new S2CLogin(){ Guid = plInfo.guid});
        }

    }

    

    
}