using C2SProtoInterface;
using GameServer;

namespace GameServer
{

    public class GameRoomAgent
    {
        public int guid { get; private set; }
        List<int> _playerGuids = new List<int>();
        
        public int playerCount => _playerGuids.Count;
        public int main => _playerGuids[0];
        MatchAgent _matchAgent = new MatchAgent();
        public bool inMatch => _matchAgent.state != MatchState.Idle;
        public GameRoomAgent(int plGuid,int guid)
        {
            _playerGuids.Clear();
            _playerGuids.Add(plGuid);
            this.guid = guid;
            Console.WriteLine($"Create Room {guid}");
        }

        public void SendPlayerRoomInfo()
        {
            if (_playerGuids.Count<=0)
            {
                return;
            }

            var roomInfo = GetRoomInfo();
            foreach (var plGuid in _playerGuids)
            {
                var agent = PlayerManager.instance.GetPlayerByGuid(plGuid);
                SendPlayerEnterRoom(roomInfo,agent);
            }
        }

        S2CRoomInfo GetRoomInfo()
        {
            var roomInfo = new S2CRoomInfo();
            roomInfo.RoomGuid = guid;
            
            foreach (var plGuid in _playerGuids)
            {
                var agent = PlayerManager.instance.GetPlayerByGuid(plGuid);
                roomInfo.AllPlayers.Add(agent.GetS2CPlayerData());
            }

            return roomInfo;
        }

        void SendPlayerEnterRoom(S2CRoomInfo roomInfo,PlayerAgent pl)
        {
            pl.SendTCPData(EMessage.S2CRoomInfoRefresh,roomInfo);
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
            bool isNewPlayer = true;
            isNewPlayer = _playerGuids.Find((guid)=>guid == plAgent.guid) == 0;

            if (isNewPlayer)
            {
                if (inMatch)
                {
                    return;
                }
                AddPlayer(plAgent.guid);
                Console.WriteLine($"Player {plAgent.name} Join Room {guid}");
            }
            
            if (inMatch)
            {
                var matchInfo = GetMatchInfo();
                PlayerStartMatch(matchInfo,plAgent);
            }
            else
            {
                if (isNewPlayer)
                {
                    SendPlayerRoomInfo();
                }
                else
                {
                    var roomInfo = GetRoomInfo();
                    SendPlayerEnterRoom(roomInfo,plAgent);
                    Console.WriteLine($"Player {plAgent.name} ReJoin Room {guid}");    
                }
            }
        }

        public void AddPlayer(int plGid)
        {
            if (!inMatch)
            {
                _playerGuids.Add(plGid);    
            }
        }

        public void RemovePlayer(int guid)
        {
            for (int i = 0; i < _playerGuids.Count; i++)
            {
                var plGid = _playerGuids[i];
                if (plGid == guid)
                {
                    _playerGuids.RemoveAt(i);
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
            foreach (var plGid in _playerGuids)
            {
                roomInfo.RoomGuid = guid;
                var agent = PlayerManager.instance.GetPlayerByGuid(plGid);
                roomInfo.AllPlayers.Add(agent.GetS2CPlayerData());
            }

            return roomInfo;
        }

        public void StartMatch()
        {
            _matchAgent.StartMatch(_playerGuids);
            var matchInfo = GetMatchInfo();
            foreach (var plGid in _playerGuids)
            {
                var pl = PlayerManager.instance.GetPlayerByGuid(plGid);
                PlayerStartMatch(matchInfo,pl);
            }
        }

        public void EndMatch()
        {
            if (inMatch)
            {
                _matchAgent.EndMatch();
                foreach (var plGid in _playerGuids)
                {
                    var pl = PlayerManager.instance.GetPlayerByGuid(plGid);
                    PlayerEndMatch(GetRoomInfo(),pl);
                }
            }
        }

        S2CMatchInfo GetMatchInfo()
        {
            var matchInfo = new S2CMatchInfo();
            matchInfo.RoomGuid = guid;
            matchInfo.Pot = ServerLogic.udpPot;
            var random = new Random();
            matchInfo.RandomSeed = random.Next(int.MinValue,int.MaxValue);
            _matchAgent.SetMatchInfo(matchInfo);

            return matchInfo;
        }

        public void PlayerStartMatch(S2CMatchInfo matchInfo,PlayerAgent pl)
        {
            pl.SendTCPData(EMessage.S2CStartMatch,matchInfo);
        }

        public void PlayerEndMatch(S2CRoomInfo roominfo,PlayerAgent pl)
        {
             pl.SendTCPData(EMessage.S2CEndMatch,roominfo);
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

