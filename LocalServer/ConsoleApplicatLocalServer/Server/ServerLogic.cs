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
        public string name;
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

    public struct PlayerFrameInput
    {
        public int guid;
        public int input;
        public long moveAngle;

        public PlayerFrameInput Init(int gid)
        {
            guid = gid;
            return this;
        }

        public void Refresh(C2SFrameUpdate c2sDt)
        {
            input = c2sDt.Input;
            moveAngle = c2sDt.Angle;
        }
    }

    public struct PlayerFrame
    {
        public int frame;
        public List<PlayerFrameInput> inputs;

        public PlayerFrame Init(int frm)
        {
            inputs = new List<PlayerFrameInput>();
            frame = frm;
            return this;
        }
    }

    public class ServerLogic
    {
        //毫秒
        public static int frameTime = 33;
        private static int playerNum = 1;
        private static int index = -1;
        private string ip = "192.168.50.22";
        private int tcpPot = 8080;
        IPEndPoint tcpIPPoint;
        private IPEndPoint udpIPPoint;
        private TcpListener tcpListener;
        private BinaryFormatter _serializer = new BinaryFormatter();
        private List<PlayerFrame> _frameInputs = new List<PlayerFrame>(10000);
        
        private Dictionary<int, PlayerInfo> allPlayers = new Dictionary<int, PlayerInfo>();
        private Dictionary<TcpClient, PlayerInfo> clientCollection = new Dictionary<TcpClient, PlayerInfo>();
        private List<TcpClient> tmpRemovedClient = new List<TcpClient>();

        private int curFrame => _frameInputs.Count -1;
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
                    var frmDt = new S2CFrameUpdate();//ServerFrameManager.GetSendFrameData(curFrame,_frameInputs[_frameInputs.Count - 1]);
                    frmDt.CurServerFrame = curFrame;
                    AddFrameData(frmDt, _frameInputs.Count - 1, _frameInputs.Count - 1);

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
                    PlayerFrame input = new PlayerFrame();
                    input.Init(_frameInputs.Count);
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
                    clientCollection.Add(tcpClient, new PlayerInfo(tcpClient));
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
            lock (clientCollection)
            {
                foreach (var kv in clientCollection)
                {
                    if (kv.Value.enabled)
                        OnRecieveTCPInfo(kv.Value);
                }

                if (tmpRemovedClient.Count > 0)
                {
                    foreach (var client in tmpRemovedClient)
                    {
                        clientCollection.Remove(client);
                    }
                }

                tmpRemovedClient.Clear();
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

            if (msgType != EMessage.EnterGame && plInfo.guid == 0)
                return;

            switch (msgType)
            {
                case EMessage.EnterGame:
                    OnPlayerLogin(plInfo, streamBuffer, sizeof(EMessage), readLen);
                    break;
                case EMessage.Restart:
                    OnRestartGame(plInfo);
                    break;
                case EMessage.PrintFrames:
                    OnPrintFrame();
                    break;
            }
        }

        void OnPlayerLogin(PlayerInfo plInfo, byte[] streamBuffer, int offset, int len)
        {
            var c2SLog = C2SLogin.Parser.ParseFrom(streamBuffer, offset, len - offset);
            var guid = c2SLog.GId;
            if (guid != 0)
            {
                if (allPlayers.TryGetValue(guid, out var oldPlInfo))
                    RemoveClient(oldPlInfo);
                else
                    plInfo.guid = guid;
                allPlayers[guid] = plInfo;
                plInfo.name = c2SLog.Name;
            }

            OnRestartGame(plInfo);
        }

        void RemoveClient(PlayerInfo rmPlInfo)
        {
            rmPlInfo.enabled = false;
            tmpRemovedClient.Add(rmPlInfo.client);
        }

        private int[] playerIds;

        void OnRestartGame(PlayerInfo plInfo)
        {
            
            lock (clientCollection)
            {
                S2CPlayerData[] pls = new S2CPlayerData[allPlayers.Count];
                int[] playerIds = new int[allPlayers.Count];
                int idx = 0;
                foreach (var kv in allPlayers)
                {
                    pls[idx] = new S2CPlayerData() { Guid = kv.Key, Name = kv.Value.name };
                    kv.Value.udpEndPoint = default;
                    playerIds[idx] = kv.Key;
                    ++idx;
                }

                foreach (var kv in allPlayers)
                {
                    var stGame = new S2CStartGame();
                    stGame.Players.AddRange(pls);
                    stGame.Pot = udpIPPoint.Port;
                    SendTCPData(kv.Value.stream, EMessage.Restart, stGame);
                }
            }
            lock (_frameInputs)
            {
                _frameInputs.Clear();
            }
        }

        void OnPrintFrame()
        {
            foreach (var kv in allPlayers)
            {
                SendTCPData(kv.Value.stream, EMessage.PrintFrames, new S2CPrintFrames());
            }
            lock (_frameInputs)
            {
                var path = Directory.GetCurrentDirectory()+"\\FramesPrint.txt";
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                using (StreamWriter c = new StreamWriter(path, true))
                {

                    for (int i = 0; i < _frameInputs.Count; i++)
                    {
                        string inputs = "";
                        var frame = _frameInputs[i];
                        foreach (var input in frame.inputs)
                        {
                            inputs += $" id:{input.guid} yaw{input.moveAngle} ";
                        }
                        c.WriteLine($"[{frame.frame}] {inputs}");
                    }
                }
            }
        }

        unsafe void UDPRecieveing()
        {
            _udpSocket.Bind(udpIPPoint);
            byte[] data = new byte[1024];
            while (true)
            {
                OnReceiveUDP(data);
                Thread.Sleep(0);
            }
        }

        unsafe void OnReceiveUDP(byte[] data)
        {
            EndPoint senderRemote = new IPEndPoint(IPAddress.Any, 0);
            int readLeng;
            try
            {
                readLeng = _udpSocket.ReceiveFrom(data, data.Length, SocketFlags.None, ref senderRemote);
            }
            catch (Exception e)
            {
                return;
            }

            lock (_frameInputs)
            {
                if (curFrame <= 0)
                    return;

                fixed (void* p = data)
                {
                    int _msgOffset = 0;
                    int guid = *(int*)p;
                    _msgOffset += sizeof(int);

                    lock (allPlayers)
                    {
                        if (allPlayers.TryGetValue(guid, out var plInfo))
                        {
                            if (plInfo.udpEndPoint != senderRemote)
                                plInfo.udpEndPoint = senderRemote;
                        }
                        else
                        {
                            return;
                        }
                    }

                    var upData = C2SFrameUpdate.Parser.ParseFrom(data, _msgOffset, readLeng - _msgOffset);

                    //丢帧补帧
                    int start = Math.Min(upData.Start, curFrame - 1);

                    if (start >= 0)
                    {
                        int end = Math.Max(upData.Start + 500, upData.End);
                        end = Math.Min(end, curFrame - 1);
                        end = Math.Max(end, start);
                        S2CFrameUpdate toC = new S2CFrameUpdate();
                        toC.CurServerFrame = curFrame - 1;

                        if (end < curFrame)
                            AddFrameData(toC, start, end);

                        byte[] sendBytes = toC.ToByteArray();
                        _udpSocket.SendTo(sendBytes, 0, sendBytes.Length, SocketFlags.None, senderRemote);
                    }

                    var idx = -1;
                    PlayerFrame frame;
                    {
                        frame = _frameInputs[_frameInputs.Count - 1];

                        var inputs = frame.inputs;
                        for (int i = 0; i < inputs.Count; i++)
                        {
                            if (inputs[i].guid == guid)
                                idx = i;
                        }

                        PlayerFrameInput curInput;
                        if (idx == -1)
                        {
                            curInput = new PlayerFrameInput();
                            idx = inputs.Count;
                            curInput.Init(guid);
                            inputs.Add(curInput);
                        }
                        else
                            curInput = inputs[idx];

                        curInput.Refresh(upData);
                        inputs[idx] = curInput;
                    }
                }
            }
        }

        void AddFrameData(S2CFrameUpdate toC, int start, int end)
        {
            start = Math.Max(0, start);
            end = Math.Min(curFrame, end);
            end = Math.Max(start, end);
            for (int sendIdx = start; sendIdx < end + 1; sendIdx++)
            {
                S2CFrameData frmDt = new S2CFrameData();
                frmDt.FrameIndex = sendIdx;
                var frm = _frameInputs[sendIdx];

                for (int i = 0; i < frm.inputs.Count; i++)
                {
                    var input = frm.inputs[i];
                    frmDt.Gids.Add(input.guid);
                    frmDt.Inputs.Add(input.input);
                    frmDt.InputAngles.Add(input.moveAngle);
                }

                toC.FrameDatas.Add(frmDt);
            }
        }
    }
}