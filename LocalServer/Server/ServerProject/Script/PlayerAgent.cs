
using C2SProtoInterface;
using Google.Protobuf;

namespace GameServer
{
    public class PlayerAgent
    {
        public TCPInfo tcpInfo { get; private set; }
        private int roomGuid = -1;
        public int guid => login?.GId??-1;
        public string name => login.Name;
        private C2SLogin login;

        public PlayerAgent(C2SLogin login)
        {
            this.login = login;
        }

        public void Init(TCPInfo tcpInfo)
        {
            this.tcpInfo = tcpInfo;
        }
        
        public void ReceiveTCPMessage(EMessage message,byte[] streamBuffer,int readLen)
        {
            switch (message)
            {
                case EMessage.C2SCreateRoom:
                    RoomManager.instance.ReqCreateRoom(this);
                    break;
                case EMessage.C2SLeaveRoom:
                    RoomManager.instance.ReqLeaveRoom(this);
                    break;
                case EMessage.C2SJoinRoom:
                    RoomManager.instance.ReqJoinRoom(this,streamBuffer,readLen);
                    break;
                case EMessage.ReqRoomInfos:
                    RoomManager.instance.ReqRoomInfo(this);
                    break;
                case EMessage.C2SStartMatch:
                    RoomManager.instance.ReqStartMatch(this);
                    break;
            }
        }

        public unsafe void SendTCPData( EMessage messageType, IMessage obj = null)
        {
            Stream stream = tcpInfo.stream;
            
            byte[] data = new byte[sizeof(EMessage) + sizeof(int) + (obj?.CalculateSize() ?? 0)];
            var infoBytes = obj?.ToByteArray()??new byte[0];
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
        
        public void OnLogin(S2CLogin login)
        {
            SendTCPData(EMessage.Login,login);
        }

        public void OnLogout()
        {
            Console.WriteLine($"Player Logout:{name} guid:{guid}");
            SendTCPData(EMessage.Logout);
            tcpInfo = null;
        }

        public S2CPlayerData GetS2CPlayerData()
        {
            return new S2CPlayerData() { Guid = guid, Name = name };
        }
    }

    public class PlayerManager : Single<PlayerManager>
    {
        Dictionary<TCPInfo,PlayerAgent> playerTcpInfos = new Dictionary<TCPInfo,PlayerAgent>();
        Dictionary<int,PlayerAgent> guidplayersContainer = new Dictionary<int,PlayerAgent>();

        public PlayerManager()
        {
            playerTcpInfos.Clear();
        }

        /// <summary>
        /// 是否玩家已经登录
        /// </summary>
        /// <param name="tcpInfo"></param>
        /// <param name="login"></param>
        /// <returns></returns>
        public TCPInfo PrlayerLogin(TCPInfo tcpInfo,C2SLogin login)
        {
            PlayerAgent player;
            TCPInfo oldTCP = null;
            lock (guidplayersContainer)
            {
                if (guidplayersContainer.TryGetValue(login.GId, out player))
                {
                    oldTCP = player.tcpInfo;
                    player.Init(tcpInfo);
                    guidplayersContainer.Remove(player.guid);
                    Console.WriteLine($"Player ReLogin:{login.Name} guid:{login.GId}");
                }
                else
                {
                    player = new PlayerAgent(login);
                    player.Init(tcpInfo);
                    Console.WriteLine($"Player Login:{login.Name} guid:{login.GId}");
                }

            
                guidplayersContainer.Add(player.guid,player);
            }
            playerTcpInfos[tcpInfo] = player;

            var loginInfo = new S2CLogin();
            var roomInfo = RoomManager.instance.GetRoomAgentByPlayerGuid(player.guid);
            if ( roomInfo!= null)
            {
                loginInfo.RoomId = roomInfo.guid;
            }
            player.OnLogin(loginInfo);
            return oldTCP;
        }

        

        public void RemovePlayerByTCPInfo(TCPInfo tcpInfo)
        {
            if (playerTcpInfos.TryGetValue(tcpInfo,out var player))
            {
                playerTcpInfos.Remove(tcpInfo);
                lock (guidplayersContainer)
                {
                    guidplayersContainer.Remove(player.guid);
                }
                player.OnLogout();
            }
        }

        public PlayerAgent GetPlayerByTCPInfo(TCPInfo tcpInfo)
        {
            if (playerTcpInfos.TryGetValue(tcpInfo,out var playerAgent))
            {
                return playerAgent;
            }

            return null;
        }

        public PlayerAgent GetPlayerByGuid(int guid)
        {
            guidplayersContainer.TryGetValue(guid, out var pl);
            return pl;
        }
        
    }
}
