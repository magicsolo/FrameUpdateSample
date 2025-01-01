using C2SProtoInterface;
using GameServer;

namespace GameServer
{
    public struct RommPlayerInfo
    {
        public int guid;
        public string name;

        public RommPlayerInfo(PlayerAgent pl)
        {
            guid = pl.guid;
            name = pl.name;
        }
    }

    public class GameRoomAgent
    {
        public int guid { get; private set; }
        List<RommPlayerInfo> _players = new List<RommPlayerInfo>();
        
        public int playerCount => _players.Count;
        public RommPlayerInfo main => _players[0];
        private bool inMatch;
        MatchAgent _matchAgent = new MatchAgent();
        public GameRoomAgent(RommPlayerInfo pl,int guid)
        {
            _players.Clear();
            _players.Add(pl);
            inMatch = false;
            this.guid = guid;
            Console.WriteLine($"Create Room {guid}");
        }

        public void SendPlayerRoomInfo()
        {
            if (_players.Count<=0)
            {
                return;
            }
            var roomInfo = new S2CRoomInfo();
            roomInfo.RoomGuid = guid;
            
            foreach (var pl in _players)
            {
                var agent = PlayerManager.instance.GetPlayerByGuid(pl.guid);
                roomInfo.AllPlayers.Add(agent.GetS2CPlayerData());
            }

            foreach (var pl in _players)
            {
                var agent = PlayerManager.instance.GetPlayerByGuid(pl.guid);
                agent.SendTCPData(EMessage.S2CRoomInfoRefresh,roomInfo);
            }
        }

        public void OnReqLeaveRoom(PlayerAgent plAgent)
        {
            int guid = plAgent.guid;
            RemovePlayer(guid);
            Console.WriteLine($"Player {plAgent.name} Leave Room {guid}");
            plAgent.SendTCPData(EMessage.S2CLeaveRoom);
        }

        public void OnReqJoinRoom(PlayerAgent plAgent)
        {
            var newPlayer = new RommPlayerInfo(plAgent);
            AddPlayer(newPlayer);
            SendPlayerRoomInfo();
            Console.WriteLine($"Player {plAgent.name} Join Room {guid}");
        }

        public void OnReceiveFrameData(int guid,C2SFrameUpdate frameData)
        {
            
        }

        public void AddPlayer(RommPlayerInfo pl)
        {
            if (!inMatch)
            {
                _players.Add(pl);    
            }
        }

        public void RemovePlayer(int guid)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                var pl = _players[i];
                if (pl.guid == guid)
                {
                    _players.RemoveAt(i);
                    break;
                }
            }

            SendPlayerRoomInfo();
        }

        public void URPGameInput()
        {
            
        }

        public S2CRoomInfo GetAllRoomInfo()
        {
            var roomInfo = new S2CRoomInfo();
            foreach (var pl in _players)
            {
                roomInfo.RoomGuid = guid;
                var agent = PlayerManager.instance.GetPlayerByGuid(pl.guid);
                roomInfo.AllPlayers.Add(agent.GetS2CPlayerData());
            }

            return roomInfo;
        }

        public void StartMatch()
        {
            _matchAgent.StartMatch(_players);
            var matchInfo = new S2CMatchInfo();
            matchInfo.RoomGuid = guid;
            matchInfo.Pot = ServerLogic.udpPot;
            foreach (var pl in _players)
            {
                var agent = PlayerManager.instance.GetPlayerByGuid(pl.guid);
                matchInfo.Players.Add(agent.GetS2CPlayerData());
            }
            foreach (var agent in _players)
            {
                var pl = PlayerManager.instance.GetPlayerByGuid(agent.guid);
                pl.SendTCPData(EMessage.S2CStartMatch,matchInfo);
            }
        }

        public void ReceiveC2SFrameData(int guid, C2SFrameUpdate data)
        {
            _matchAgent.ReceiveC2SFrameData(guid, data);
        }

        public void Update()
        {
            _matchAgent.Update();
        }
    }
}

