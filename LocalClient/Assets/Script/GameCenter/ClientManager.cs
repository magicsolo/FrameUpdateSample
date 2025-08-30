using System;
using System.Collections;
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

    public class TCPListener
    {
        Dictionary<EMessage,Action<TCPInfo>> _handlers = new Dictionary<EMessage, Action<TCPInfo>>();

        public void AddHandler(EMessage key, Action<TCPInfo> handler)
        {
            _handlers.Add(key, handler);
        }

        public void RegistAll()
        {
            foreach (var kv in _handlers)
            {
                ClientManager.instance.RegistNoteListener(kv.Key,kv.Value);
            }
        }

        public void UnRegistAll()
        {
            foreach (var kv in _handlers)
            {
                ClientManager.instance.UnRegistNoteListener(kv.Key);
            }
            _handlers.Clear();
        }
    }

    public class ClientManager : BasicMonoSingle<ClientManager>
    {
        private Dictionary<EMessage, Action<TCPInfo>> _callBacks = new Dictionary<EMessage, Action<TCPInfo>>();

        public string ip = "192.168.50.23";
        public string pot = "8080";
        public int guid { get; private set; }
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
            

            tcp = new TCPConnecter(onConnectionEnd:OnConnectionEnd);
            udp = new UDPConnecter();
        }

        public void LoadIPInfo()
        {
            ip = PlayerPrefs.GetString("ip");
            pot = PlayerPrefs.GetString("pot");
        }

        public void SetDefaultIP()
        { 
            ip = "192.168.50.76";
            pot = "8090";
        }
        
        public void Connect()
        {
            tcp.Connect(ip, pot);
            PlayerPrefs.SetString("ip",ip);
            PlayerPrefs.SetString("pot",pot);
            Init();
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

            var readLength = 0;
            try
            {
                readLength = tcp.Receive(readBytes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            if (readLength <= 0)
                return;
            RecieveTCPInfo(readBytes);
        }

        void RecieveTCPInfo(byte[] data)
        {
            if (data == null || data.Length < 1)
            {
                return;
            }
            var tcpData = new TCPInfo();
            tcpData.Init(data);

            if (_callBacks.TryGetValue(tcpData.msgType, out var func))
                func(tcpData);
        }

        public void Login(Action<TCPInfo> OnLogin = null)
        {
            guid = playerName.GetHashCode();
            PlayerPrefs.SetString("name",playerName);
            PlayerPrefs.SetInt("guid",guid);
            SendTCPInfo(EMessage.Login, new C2SLogin() { Name = playerName, GId = guid },OnLogin);
        }

        public void LogOut()
        {
            SendTCPInfo(EMessage.Logout, null,OnLogOut);
        }

        private void OnLogOut(TCPInfo obj)
        {
            OnConnectionEnd();
        }
        
        
        private void OnConnectionEnd()
        {
            udp.Close();
            tcp.Close();
            EventManager.instance.DispatchEvent(EventKeys.OnOffLine);

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
            var infoBytes = data?.ToByteArray();
            if (infoBytes!=null)
                Buffer.BlockCopy(infoBytes, 0, bytes, sizeof(EMessage), infoBytes.Length);
            try
            {
                tcp.SendData(bytes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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
            _callBacks.Remove(mesgType);
        }
        
        
    }
}