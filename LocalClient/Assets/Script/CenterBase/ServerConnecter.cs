//#define  CheckOutLine
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using C2SProtoInterface;
using UnityEngine;

namespace CenterBase
{
    public enum EConnecterState
    {
        DisConnect,
        Connecting,
        Connected,
    }

    public abstract class BaseConnecter
    {
        public abstract EConnecterState CurState { get; }
        protected int pot;

        protected string ip;

        // IPAddress ipAddress;
        protected IPEndPoint edPoint;
        public NetworkStream Stream { get; protected set; }


        public virtual void Connect(string ip, string pot)
        {
            Close();

            Connect(ip,Convert.ToInt32(pot));
        }

        public virtual void Connect(String ip, int pot)
        {
            this.ip = ip;
            this.pot = pot;
            IPAddress ipAddress = IPAddress.Parse(ip);
            edPoint = new IPEndPoint(ipAddress, this.pot);
        }

        public abstract void Close();
    }

    public class UDPConnecter : BaseConnecter
    {
        //public EConnecterState CurState=>;
        private Socket udpSocket;

        byte[] receiveDatas = new byte[600000];
        public override EConnecterState CurState => (udpSocket == null || !udpSocket.Connected) ? EConnecterState.DisConnect : EConnecterState.Connected;

        public override void Connect(string ip, string pot)
        {
            base.Connect(ip, pot);
            SocketConnect();
        }

        public override void Connect(string ip, int pot)
        {
            base.Connect(ip, pot);
            SocketConnect();
        }

        void SocketConnect()
        {
            udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udpSocket.Connect(edPoint);
        }

        public override void Close()
        {
            if (udpSocket != null)
            {
                udpSocket.Close();
                udpSocket = null;
            }
        }

        public void Send(byte[] byts)
        {
            udpSocket.Send(byts);
        }

        public S2CFrameUpdate Receive()
        {
            if (udpSocket.Available > 0)
            {
                int len = udpSocket.Receive(receiveDatas, receiveDatas.Length, SocketFlags.None);
                var s2CData = S2CFrameUpdate.Parser.ParseFrom(receiveDatas, 0, len);
                return s2CData;

            }

            return null;
        }
    }

    public class TCPConnecter : BaseConnecter
    {
        private EConnecterState _state;
        private EConnecterState _curState
        {
            get => _state;
            set
            {
                if (_state == EConnecterState.DisConnect)
                {
                    return;
                }

                _state = value;
            }
        }
        public override EConnecterState CurState { get => _curState; }

        private TcpClient tcp;

        private Thread thrdConnect;
        private Action connectionEnd;
        private int heartSpaceTime;
        private long heartTickBeatCheckSpaceTime => heartSpaceTime * 10000000;
        private long disconnectTickSpaceTime;
        private long lastRecvTime;

        public TCPConnecter(int heartCheckTime = 1,int disconnectSpaceTime = 10,Action onConnectionEnd = null)
        {
            _curState = EConnecterState.DisConnect;
            heartSpaceTime = heartCheckTime;
            this.disconnectTickSpaceTime = disconnectSpaceTime * 10000000;
            connectionEnd = onConnectionEnd;
        }
        
        public override void Close()
        {
            CloseTCP();
        }

        void CloseTCP()
        {
            if (tcp != null)
            {
                lock (tcp)
                {
                    tcp.Close();
                    tcp = null;    
                    _curState = EConnecterState.DisConnect;
                }
            }
            else
            {
                _curState = EConnecterState.DisConnect;
            }
        }
        
        public override void Connect(string ip, string pot)
        {
            base.Connect(ip, pot);
            thrdConnect = new Thread(TCPConnect);
            _state = EConnecterState.Connecting;
            thrdConnect.Start();
        }
        
        void TCPConnect()
        {
            tcp = new TcpClient();

            try
            {
                tcp.Connect(edPoint);
            }
            catch (Exception e)
            {
                CloseTCP();
                Console.WriteLine(e);
                return;
            }

            Stream = tcp.GetStream();
            _curState = EConnecterState.Connected;
            lastRecvTime = DateTime.Now.Ticks;
#if CheckOutLine
            while (_curState == EConnecterState.Connected)
            {
                var spaceTime = DateTime.Now.Ticks - lastRecvTime; 
                if (spaceTime > disconnectTickSpaceTime)
                {
                    if (connectionEnd!=null)
                    {
                        connectionEnd();
                    }
                    return;
                }else if (spaceTime > heartTickBeatCheckSpaceTime)
                {
                    SendData(new byte[] { 1 });
                }
                Thread.Sleep(this.heartSpaceTime *1000);
            }
#endif
            
        }

        public void SendData(byte[] byts)
        {
            if (tcp == null)
            {
                return;
            }
            
            lock (tcp)
            {
                if (_curState!= EConnecterState.Connected)
                {
                    return;
                }
                
                try
                {
                    Stream.Write(byts, 0, byts.Length);
                }
                catch (Exception e)
                {
                    if (connectionEnd!=null)
                    {
                        connectionEnd();
                    }
                }
            }
        }

        public int Receive(byte[] byts)
        {
            lock (tcp)
            {
                if (_curState!= EConnecterState.Connected)
                {
                    return 0;
                }
                if (!Stream.DataAvailable)
                {
                    return 0;
                }
                var len = Stream.Read(byts, 0, byts.Length);
                if (len > 0)
                {
                    lastRecvTime = DateTime.Now.Ticks;
                }
                return len;
            }
            
        }
        

    }
}