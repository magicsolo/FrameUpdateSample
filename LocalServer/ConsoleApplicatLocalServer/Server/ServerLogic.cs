using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using C2SProtoInterface;
using Google.Protobuf;
using Google.Protobuf.Collections;

namespace ConsoleApplicatLocalServer
{
    class PlayerInfo
    {
        public int index = -1;
        public int guid = -1;
        public float lastHeartTime = -1;
        public TcpClient client;
        public NetworkStream stream;
        public EndPoint udpEndPoint;
        public bool enabled;

        public PlayerInfo(TcpClient cl)
        {
            guid = -1;
            client = cl;
            stream = cl.GetStream();
            enabled = true;
        }
    }

    struct PlayerFrameInput
    {
        public int input;
        public long moveAngle;

        public void Init()
        {
            moveAngle = -1;
        }
    }
    
    public class ServerLogic
    {
        //毫秒
        public static int frameTime = 333;
        private static int playerNum = 1;
        private static int index = -1;
        private string ip = "192.168.50.22";
        private int tcpPot = 8080;
        IPEndPoint tcpIPPoint;
        private IPEndPoint udpIPPoint;
        private TcpListener tcpListener;
        private BinaryFormatter _serializer = new BinaryFormatter();
        private List<PlayerFrameInput> _frameInputs = new List<PlayerFrameInput>();
        private Dictionary<int, PlayerInfo> allPlayers = new Dictionary<int, PlayerInfo>();
        private Dictionary<TcpClient,PlayerInfo> clientCollection = new Dictionary<TcpClient,PlayerInfo>();
        private List<TcpClient> tmpRemovedClient = new List<TcpClient>();

        private int curFrame => _frameInputs.Count / playerNum - 1;
        private Socket _udpSocket;

        public void StartServer()
        {
            allPlayers = new Dictionary<int, PlayerInfo>();

            IPAddress ipAdress = IPAddress.Parse(ip);
            tcpIPPoint = new IPEndPoint(ipAdress, tcpPot);
            Console.WriteLine($"tcpIp:{tcpIPPoint}");
            _udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udpIPPoint = new IPEndPoint(ipAdress, GetUDPPort());

            Console.WriteLine($"udpIp:{udpIPPoint}");
            tcpListener = new TcpListener(tcpIPPoint);
            Thread collect = new Thread(Collecting);
            Thread udpRecieveing = new Thread(UDPRecieveing);
            collect.Start();
            udpRecieveing.Start();
        }

        //返回  0：失败   startPort~endPort：成功
        private int GetUDPPort(int startPort = 1000, int endPort = 65535)
        {
            int port = startPort;

            IPGlobalProperties ipGlobalProperties =
                IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] endPoints =
                ipGlobalProperties.GetActiveUdpListeners();
            HashSet<int> usedPoint = new HashSet<int>();

            foreach (IPEndPoint endPoint in endPoints)
                usedPoint.Add(endPoint.Port);

            while (port <= endPort)
            {
                port++;
                if (!usedPoint.Contains(port))
                {
                    return port;
                }
            }

            return 0;
        }


        public void Update()
        {
            lock (_frameInputs)
            {
                if (curFrame > 0)
                {
                    S2CFrameUpdate frmDt = new S2CFrameUpdate();
                    frmDt.CurServerFrame = curFrame;
                    AddFrameData(frmDt, curFrame, curFrame);
                    byte[] sendBytes = frmDt.ToByteArray();

                    foreach (var plInfo in allPlayers.Values)
                        if (plInfo.udpEndPoint != null)
                        {
                            _udpSocket.SendTo(sendBytes, 0, sendBytes.Length, SocketFlags.None, plInfo.udpEndPoint);
                            //Console.WriteLine($"发送当前帧 {curFrame} {DateTime.Now}");
                        }
                }
                
                for (int i = 0; i < playerNum; i++)
                {
                    PlayerFrameInput input = new PlayerFrameInput();
                    input.Init();
                    _frameInputs.Add(input);
                }
            }
        }

        public void Collecting()
        {
            tcpListener.Start();
            while (true)
            {
                var tcpClient = tcpListener.AcceptTcpClient();
                lock (clientCollection)
                {
                    clientCollection.Add(tcpClient,new PlayerInfo(tcpClient));
                }
            }
        }

        unsafe void SendTCPData(Stream stream, EMessage messageType, IMessage obj)
        {
            byte[] data = new byte[sizeof(EMessage) + sizeof(int) + obj.CalculateSize()]; //` Encoding.UTF8.GetBytes(jsn);
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
                lock (clientCollection)
                {
                    foreach (var kv in clientCollection)
                    {
                        if (kv.Value.enabled)
                            OnRecieveTCPInfo(kv.Value);
                    }

                    if (tmpRemovedClient.Count>0)
                    {
                        foreach (var client in tmpRemovedClient)
                        {
                            clientCollection.Remove(client);
                        }
                    }
                    tmpRemovedClient.Clear();
                }
            }
        }

        unsafe void OnRecieveTCPInfo(PlayerInfo plInfo)
        {
            byte[] streamBuffer = new byte[1024 * 1024];

            var stream = plInfo.stream;
            if (stream == null || !stream.DataAvailable)
                return;

            var readLen = stream.Read(streamBuffer, 0, streamBuffer.Length);
            if (readLen <= 0)
                return;
            EMessage msgType = default;
            fixed (void* p = streamBuffer)
                msgType = *(EMessage*)p;

            if (msgType != EMessage.Login && plInfo.guid < 0)
                return;
            
            switch (msgType)
            {
                case EMessage.Login:
                    OnPlayerLogin(plInfo, streamBuffer,sizeof(EMessage),readLen);
                    break;
                case EMessage.StartGame:
                    OnStartGame(plInfo);
                    break;
            }
        }

        void OnPlayerLogin(PlayerInfo plInfo, byte[] streamBuffer,int offset,int len)
        {
            var c2SLog = C2SLogin.Parser.ParseFrom(streamBuffer, offset, len - offset);
            var guid = c2SLog.GId;
            if (guid > 0)
            {
                if (allPlayers.TryGetValue(guid, out var oldPlInfo))
                    RemoveClient(oldPlInfo);
                else
                    plInfo.guid = guid;
                allPlayers[guid] = plInfo;
                SendTCPData(plInfo.stream, EMessage.Login, new S2CLogin() {GId = plInfo.guid, UdpPot = udpIPPoint.Port, PlayerNum = playerNum });
            }
            
        }

        void RemoveClient(PlayerInfo rmPlInfo)
        {
            rmPlInfo.enabled = false;
            tmpRemovedClient.Add(rmPlInfo.client);
        }

        void OnStartGame(PlayerInfo plInfo)
        {
            SendTCPData(plInfo.stream, EMessage.StartGame, new S2CStartGame() { PlayerNum = playerNum });
        }

        unsafe void UDPRecieveing()
        {
            _udpSocket.Bind(udpIPPoint);
            byte[] data = new byte[1024];
            while (true)
            {
                if (curFrame <= 0)
                    continue;

                EndPoint senderRemote = new IPEndPoint(IPAddress.Any, 0);
                int readLeng = _udpSocket.ReceiveFrom(data, data.Length, SocketFlags.None, ref senderRemote);

                fixed (void* p = data)
                {
                    int _msgOffset = 0;
                    int guid = *(int*)p;
                    _msgOffset += sizeof(int);

                    lock (allPlayers)
                    {
                        var plInfo = allPlayers[guid];
                        if (plInfo.udpEndPoint != senderRemote)
                            plInfo.udpEndPoint = senderRemote;
                    }
                    
                    lock (_frameInputs)
                    {
                        var upData = C2SFrameUpdate.Parser.ParseFrom(data, _msgOffset, readLeng - _msgOffset);

                        //丢帧补帧
                        int start = Math.Min(upData.Start, curFrame - 1);
                        
                        if (start >= 0)
                        {
                            int end = Math.Min(upData.Start + 500, upData.End);
                            end = Math.Min(end, curFrame - 1);
                            end = Math.Min(end, start);
                            S2CFrameUpdate toC = new S2CFrameUpdate();
                            toC.CurServerFrame = curFrame - 1;

                            if (end < curFrame)
                                AddFrameData(toC, start, end);
                        
                            byte[] sendBytes = toC.ToByteArray();
                            _udpSocket.SendTo(sendBytes, 0, sendBytes.Length, SocketFlags.None, senderRemote);
                            Console.WriteLine($"补发帧 {start} - {end} {DateTime.Now}");
                        }

                        var idx = upData.Index;
                        var input = _frameInputs[curFrame * playerNum + idx];
                        input.moveAngle = upData.Angle;
                        input.input = upData.Input;
                        _frameInputs[curFrame * playerNum + idx] = input;
                    }
                }
            }
        }

        void AddFrameData(S2CFrameUpdate toC,int start,int end)
        {
            start = Math.Max(0, start);
            end = Math.Min(curFrame, end);
            end = Math.Max(start, end);
            for (int sendIdx = start; sendIdx < end + 1; sendIdx++)
            {
                S2CFrameData frmDt = new S2CFrameData();
                frmDt.FrameIndex = sendIdx;

                for (int playerIdx = 0; playerIdx < playerNum; playerIdx++)
                {
                    PlayerFrameInput frmInput = _frameInputs[sendIdx + playerIdx];
                    frmDt.Inputs.Add(frmInput.input);
                    frmDt.InputAngles.Add(frmInput.moveAngle);
                }

                toC.FrameDatas.Add(frmDt);
            }
        }
    }
}