using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using C2SProtoInterface;
using CenterBase;
using TrueSync;
using UnityEngine;
using Google.Protobuf;

namespace Game
{
    public enum EInputEnum
    {
        none,
    }

    public struct TCPInfo
    {
        private Byte[] _datas;
        public EMessage msgType { get; private set; }
        int _msgOffset;
        int _msgLength;

        public unsafe void Init(Byte[] datas)
        {
            _datas = datas;
            _msgOffset = 0;
            fixed (byte* p = datas)
            {
                msgType = *(EMessage*)p;
                _msgOffset += sizeof(EMessage);
                _msgLength = *(int*)(p + _msgOffset);
                _msgOffset += sizeof(int);
            }
        }

        public T ParseMsgData<T>(MessageParser<T> parser) where T : IMessage<T>
        {
            return parser.ParseFrom(_datas, _msgOffset, _msgLength);
        }
    }

    public class ClientManager : BasicMonoSingle<ClientManager>
    {
        private Dictionary<EMessage, Action<TCPInfo>> _callBacks = new Dictionary<EMessage, Action<TCPInfo>>();
        private FP frameTime => FrameManager.instance.frameTime;

        public string ip = "192.168.50.22";
        public string pot = "8080";
        private int guid;
        public string playerName;
        private BinaryFormatter _serializer = new BinaryFormatter();

        private IPEndPoint udpPoint;

        //private TcpClient tcpClient;
        private TCPConnecter tcp;
        private UDPConnecter udp;
        public EConnecterState tcpState => tcp.CurState;


        private void OnEnable()
        {
            playerName = PlayerPrefs.GetString("name");
            guid = PlayerPrefs.GetInt("guid");
            ip = PlayerPrefs.GetString("ip");
            pot = PlayerPrefs.GetString("pot");

            tcp = new TCPConnecter();
            udp = new UDPConnecter();
        }

        public void ReConnect()
        {
            IPAddress ipAddress = IPAddress.Parse(ip);
            tcp.Connect(ip, pot);
            PlayerPrefs.SetString("ip",ip);
            PlayerPrefs.SetString("pot",pot);
            Init();
            // Thread TCP = new Thread(TCPUpdate);
            // TCP.Start();
        }

        private void Update()
        {
            TCPUpdate();
        }

        void TCPUpdate()
        {
            var readBytes = new byte[1024];
            if (tcp.CurState != EConnecterState.Connected)
                return;

            if (!tcp.Stream.DataAvailable)
                return;
            
            var readLength = tcp.Stream.Read(readBytes, 0, readBytes.Length);
            if (readLength <= 0)
                return;
            RecieveTCPInfo(readBytes);
        }

        void RecieveTCPInfo(byte[] data)
        {
            var tcpData = new TCPInfo();
            tcpData.Init(data);

            if (_callBacks.TryGetValue(tcpData.msgType, out var func))
                func(tcpData);
        }

        public void Login()
        {
            guid = playerName.GetHashCode();
            PlayerPrefs.SetString("name",playerName);
            PlayerPrefs.SetInt("guid",guid);
            ClientManager.instance.SendTCPInfo(EMessage.EnterGame, new C2SLogin() { Name = playerName, GId = guid });
            // ,
            // (data) =>
            // {
            //     var logInfo = data.ParseMsgData(S2CLogin.Parser);
            //     callBack(logInfo);
            // });
        }


        public unsafe void SendTCPInfo(EMessage mesgType, IMessage data = null, Action<TCPInfo> callBack = null)
        {
            if (callBack != null)
            {
                _callBacks[mesgType] = callBack;
            }

            var bytes = new Byte[sizeof(EMessage) + (data == null ? 0 : data.CalculateSize())];
            fixed (void* pbytes = bytes)
                *(EMessage*)pbytes = mesgType;
            var infoBytes = data.ToByteArray();
            Buffer.BlockCopy(infoBytes, 0, bytes, sizeof(EMessage), infoBytes.Length);
            tcp.Stream.Write(bytes, 0, bytes.Length);
        }

        private void Init()
        {
        }

        public void UDPConnect(int pot)
        {
            udp.Connect(ip,pot);
        }
        

        public unsafe void UDPSend(C2SFrameUpdate data)
        {
            var dataBytes = data.ToByteArray();
            var sendBytes = new Byte[sizeof(Int32) + (data == null ? 0 : data.CalculateSize())];
            fixed (void* pbytes = sendBytes)
                *(Int32*)pbytes = guid;
            Buffer.BlockCopy(dataBytes, 0, sendBytes, sizeof(Int32), dataBytes.Length);
            udp.Send(sendBytes);
        }

        public S2CFrameUpdate UDPReceive()
        {
            return udp.Receive();
        }

        private void OnDestroy()
        {
            tcp.Close();
        }

        public void RegistNoteListener(EMessage mesgType,Action<TCPInfo> callBack)
        {
            _callBacks[mesgType] = callBack;
        }

        public void UnRegistNoteListener(EMessage mesgType)
        {
            _callBacks[mesgType] = null;
        }
    }
}