using C2SProtoInterface;
using GameServer;

namespace GameServer
{

    public class GameRoomAgent:BaseAgent
    {
        public int guid { get; private set; }
        List<int> _playerGuids = new List<int>();
        
        public int playerCount => _playerGuids.Count;
        public int main => _playerGuids[0];
        public GameRoomAgent(int guid)
        {
            _playerGuids.Clear();
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

        public S2CRoomInfo GetRoomInfo()
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
                
                AddPlayer(plAgent.guid);
                Console.WriteLine($"Player {plAgent.name} Join Room {guid}");
                SendPlayerRoomInfo();

            }
            else
            {
                var roomInfo = GetRoomInfo();
                SendPlayerEnterRoom(roomInfo,plAgent);
                Console.WriteLine($"Player {plAgent.name} ReJoin Room {guid}");   
            }
        }

        public void AddPlayer(int plGid)
        {
            _playerGuids.Add(plGid);    
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

            if (_playerGuids.Count > 0)
            {
                SendPlayerRoomInfo();
            }
            else
            {
                RoomManager.instance.RemoveRoom(guid);
            }
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
            var matchInfo = new MatchInfo();
            matchInfo.players = new MatchPlayerInfo[_playerGuids.Count];
            for (int i = 0; i < _playerGuids.Count; i++)
            {
                var plGid = _playerGuids[i];
                var pl = PlayerManager.instance.GetPlayerByGuid(plGid);
                matchInfo.players[i] = new MatchPlayerInfo(){guid = pl.guid, name = pl.name};
            }
            MatchManager.instance.StartMatch(matchInfo);
            RoomManager.instance.RemoveRoom(guid);
            
            // var matchInfo = GetMatchInfo();
            // foreach (var plGid in _playerGuids)
            // {
            //     var pl = PlayerManager.instance.GetPlayerByGuid(plGid);
            //     PlayerStartMatch(matchInfo,pl);
            // }
        }

        public void EnterFromMatch()
        {
            for (int i = 0; i < _playerGuids.Count(); i++)
            {
                var plGid = _playerGuids[i];
                
                var pl = PlayerManager.instance.GetPlayerByGuid(plGid);
                if (pl == null)
                    _playerGuids.RemoveAt(i);
            }

            if (_playerGuids.Count<=0)
            {
                RoomManager.instance.RemoveRoom(guid);
            }
            else
            {
                var roomInfo = GetAllRoomInfo();
                foreach (var plGid in _playerGuids)
                {
                    SendPlayerTCP(plGid,EMessage.S2CEndMatch, roomInfo);
                }
            }
        }

        // S2CMatchInfo GetMatchInfo()
        // {
        
        // }

        

        public void PlayerEndMatch(S2CRoomInfo roominfo,PlayerAgent pl)
        {
        }

        // public void ReceiveC2SFrameData(int guid, C2SFrameUpdate data)
        // {
        //     _matchAgent.ReceiveC2SFrameData(guid, data);
        // }
        //
        // public void Update()
        // {
        //     _matchAgent.Update();
        // }
        
    }
}

