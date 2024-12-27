//#define CheckOutLine

using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using C2SProtoInterface;
using Google.Protobuf;

namespace GameServer
{
    public class TCPInfo
    {
        public long lastHeartTime = -1;
        public TcpClient client;
        public NetworkStream stream;
        public EndPoint udpEndPoint;
        public bool enabled;

        public TCPInfo(TcpClient cl)
        {
            client = cl;
            lastHeartTime = DateTime.Now.Ticks;
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
        private const int outlineTimeSeconds = 10;
        //毫秒
        public static int frameTime = 33;
        private static int playerNum = 1;
        private static int index = -1;
        private string ip = "0.0.0.0";
        private int tcpPot = 8090;
        private int udpPot = 8091;
        IPEndPoint tcpIPPoint;
        private IPEndPoint udpIPPoint;
        private TcpListener tcpListener;
        private List<PlayerFrame> _frameInputs = new List<PlayerFrame>(10000);
        public StreamWriter videoWriter;
        public StreamWriter logWriter;
        
        private Dictionary<int, TCPInfo> allPlayers = new Dictionary<int, TCPInfo>();
        private Dictionary<TcpClient, TCPInfo> clientCollection = new Dictionary<TcpClient, TCPInfo>();
        private List<TcpClient> tmpRemovedClient = new List<TcpClient>();

        private int curFrame => _frameInputs.Count -1;
        private Socket _udpSocket;
        private bool matchStarted;

        public ServerLogic()
        {
            
        }

        public void StartServer(string inputIp)
        {
            allPlayers = new Dictionary<int, TCPInfo>();

            if (!string.IsNullOrEmpty(inputIp))
            {
                ip = inputIp;
            }
            Console.WriteLine($"InputIP:{ip}");
            IPAddress ipAdress = IPAddress.Parse(ip);
            tcpIPPoint = new IPEndPoint(ipAdress, tcpPot);
            _udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udpIPPoint = new IPEndPoint(ipAdress, udpPot);

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
                //Console.WriteLine($"当前帧 {curFrame} {DateTime.Now}");
                ///等待玩家就位
                if (!matchStarted)
                {
                    //Console.WriteLine($"记录当前帧");

                    foreach (var plInfo in allPlayers.Values)
                        if (plInfo.udpEndPoint == null)
                            return;
                    string videoPath = Directory.GetCurrentDirectory() + "\\..\\SaveData";
                    Directory.CreateDirectory(videoPath);
                    videoPath += "\\video.bytes";
                    if (videoWriter!=null)
                        videoWriter.Close();
                    if (Directory.Exists(videoPath))
                    {
                        File.Delete(videoPath);
                        File.Create(videoPath).Dispose();
                    }
                    
                    
                    videoWriter = new StreamWriter(videoPath);

                    string logPath = Directory.GetCurrentDirectory() + "\\..\\SaveData\\log.txt";
                    if (logWriter!=null)
                    {
                        logWriter.Close();   
                    }
                    File.Delete(logPath);
                    File.Create(logPath).Dispose();
                    logWriter = new StreamWriter(logPath);
                    
                    matchStarted = true;
                    //Console.WriteLine($"记录结束");
                }

                if (matchStarted && curFrame >= 0)
                {
                    //Console.WriteLine($"发送");
                    var frmDt = new S2CFrameUpdate();//ServerFrameManager.GetSendFrameData(curFrame,_frameInputs[_frameInputs.Count - 1]);
                    frmDt.CurServerFrame = curFrame;
                    AddFrameData(frmDt, _frameInputs.Count - 1, _frameInputs.Count - 1);
                    byte[] sendBytes = frmDt.ToByteArray();
                    SaveFrameBytes(frmDt);
                    foreach (var plInfo in allPlayers.Values)
                        if (plInfo.udpEndPoint != null)
                        {
                            _udpSocket.SendTo(sendBytes, 0, sendBytes.Length, SocketFlags.None, plInfo.udpEndPoint);
                            //Console.WriteLine($"向 {plInfo.guid} 发送");
                        }
                    //Console.WriteLine($"发送结束");
                }

                PlayerFrame input = new PlayerFrame();
                input.Init(_frameInputs.Count);
                _frameInputs.Add(input);
            }
            
        }

        private void SaveFrameBytes(S2CFrameUpdate frmDt)
        {
            var sendBytes = frmDt.FrameDatas[0].ToByteArray();
            var saveBytes = System.Text.Encoding.UTF8.GetString(sendBytes);
            videoWriter.WriteLine(saveBytes);
            videoWriter.Flush();

            var frmUpdate = frmDt.FrameDatas[0];
            logWriter.WriteLine($"frm:{frmUpdate.FrameIndex}");
            for (int i = 0; i < frmUpdate.Gids.Count; i++)
            {
                var gid = frmUpdate.Gids[i];
                var input = frmUpdate.Inputs[i];
                var angle = frmUpdate.InputAngles[i];
                logWriter.WriteLine($"{gid}:: input:{input} angle:{angle}");
            }
            logWriter.Flush();
        }

        public void Collecting()
        {
            Console.WriteLine($"tcpListener Collecting start {tcpIPPoint.ToString()}");
            tcpListener.Start();
            while (true)
            {
                var tcpClient = tcpListener.AcceptTcpClient();
                lock (clientCollection)
                {
                    clientCollection.Add(tcpClient, new TCPInfo(tcpClient));
                    Console.WriteLine($"Add Collect {tcpClient.Client.RemoteEndPoint}");
                }
            }
        }

        

        public void UpdateTCPInfo()
        {
            lock (clientCollection)
            {
                foreach (var kv in clientCollection)
                {
                    var plInfo = kv.Value;
                    if (plInfo.enabled)
                        OnRecieveTCPInfo(kv.Value);
                    CheckPlayerOffline(plInfo);
                }

                if (tmpRemovedClient.Count > 0)
                {
                    foreach (var client in tmpRemovedClient)
                    {
                        clientCollection.Remove(client);
                        Console.WriteLine($"TCP DisConnected:{client.Client.RemoteEndPoint}");
                    }
                }

                tmpRemovedClient.Clear();
            }
        }

        unsafe void OnRecieveTCPInfo(TCPInfo plInfo)
        {
            
            byte[] streamBuffer = new byte[1024 * 1024];

            var stream = plInfo.stream;
            if (stream == null || !stream.DataAvailable)
                return;

            var readLen = stream.Read(streamBuffer, 0, streamBuffer.Length);
            if (readLen <= 0)
                return;
            plInfo.lastHeartTime = DateTime.Now.Ticks;

            if (readLen == 1)
            {
                switch (streamBuffer[0])
                {
                    case 1:
                        streamBuffer[0] = 1;
                        stream.Write(streamBuffer, 0, 1);
                        break;
                }
                return;
            }
            
            EMessage msgType = default;
            fixed (void* p = streamBuffer)
                msgType = *(EMessage*)p;

            
            switch (msgType)
            {
                case EMessage.Login:
                    OnPlayerLogin(plInfo, streamBuffer, sizeof(EMessage), readLen);
                    break;
                case EMessage.Logout:
                    OnPlayerLogout(plInfo);
                    break;
                // case EMessage.EnterGame:
                //     
                //     break;
                // case EMessage.Restart:
                //     OnRestartGame(plInfo);
                //     break;
                default:
                    var player = PlayerManager.instance.GetPlayerByTCPInfo(plInfo);
                    if (player != null)
                    {
                        player.ReceiveTCPMessage(msgType,streamBuffer, readLen);
                    }
                    break;
            }
        }

        void OnPlayerLogin(TCPInfo tcpInfo, byte[] streamBuffer, int offset, int len)
        {
            var c2SLog = C2SLogin.Parser.ParseFrom(streamBuffer, offset, len - offset);
            Console.WriteLine($"TCP Connected:{tcpInfo.client.Client.RemoteEndPoint}");
            var oldTCP = PlayerManager.instance.PrlayerLogin(tcpInfo, c2SLog); 
            RemoveClient(oldTCP);
        }

        void OnPlayerLogout(TCPInfo plInfo)
        {
            PlayerManager.instance.RemovePlayerByTCPInfo(plInfo);
        }
        void RemoveClient(TCPInfo rmPlInfo)
        {
            if (rmPlInfo == null)
                return;
            rmPlInfo.enabled = false;
            tmpRemovedClient.Add(rmPlInfo.client);
        }

        private int[] playerIds;

        // void OnRestartGame(TCPInfo plInfo)
        // {
        //     lock (clientCollection)
        //     {
        //         S2CPlayerData[] pls = new S2CPlayerData[allPlayers.Count];
        //         int[] playerIds = new int[allPlayers.Count];
        //         int idx = 0;
        //         foreach (var kv in allPlayers)
        //         {
        //             pls[idx] = new S2CPlayerData() { Guid = kv.Key, Name = kv.Value.name };
        //             kv.Value.udpEndPoint = default;
        //             playerIds[idx] = kv.Key;
        //             ++idx;
        //         }
        //
        //         foreach (var kv in allPlayers)
        //         {
        //             var stGame = new S2CStartGame();
        //             stGame.Players.AddRange(pls);
        //             stGame.Pot = udpIPPoint.Port;
        //             SendTCPData(kv.Value.stream, EMessage.Restart, stGame);
        //         }
        //
        //         matchStarted = false;
        //     }
        //     lock (_frameInputs)
        //     {
        //         _frameInputs.Clear();
        //     }
        // }

        // void OnPrintFrame()
        // {
        //     foreach (var kv in allPlayers)
        //     {
        //         SendTCPData(kv.Value.stream, EMessage.PrintFrames, new S2CPrintFrames());
        //     }
        //     lock (_frameInputs)
        //     {
        //         var path = Directory.GetCurrentDirectory()+"\\FramesPrint.txt";
        //         if (File.Exists(path))
        //         {
        //             File.Delete(path);
        //         }
        //         using (StreamWriter c = new StreamWriter(path, true))
        //         {
        //
        //             for (int i = 0; i < _frameInputs.Count; i++)
        //             {
        //                 string inputs = "";
        //                 var frame = _frameInputs[i];
        //                 foreach (var input in frame.inputs)
        //                 {
        //                     inputs += $" id:{input.guid} yaw{input.moveAngle} ";
        //                 }
        //                 c.WriteLine($"[{frame.frame}] {inputs}");
        //             }
        //         }
        //     }
        // }

        unsafe void UDPRecieveing()
        {
            Console.WriteLine($"UDP start {udpIPPoint.ToString()}");
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
                Console.WriteLine($"UDP 收到错误信息{e.ToString()}");
                return;
            }

            lock (_frameInputs)
            {
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

                    if ( curFrame <= 0)
                        return;
                    
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

        //超时检测
        [Conditional("CheckOutLine")]
        void CheckPlayerOffline(TCPInfo player)
        {
            if (DateTime.Now.Ticks - player.lastHeartTime >outlineTimeSeconds * 10000000)
            {
                OnPlayerLogout(player);
            }
        }
    }
}