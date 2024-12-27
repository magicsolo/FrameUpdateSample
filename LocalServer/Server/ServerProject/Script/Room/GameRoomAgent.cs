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
        List<RommPlayerInfo> players = new List<RommPlayerInfo>();
        public int playerCount => players.Count;
        public RommPlayerInfo main => players[0];
        private bool inMatch;
        public GameRoomAgent(RommPlayerInfo pl,int guid)
        {
            players.Clear();
            players.Add(pl);
            inMatch = false;
            this.guid = guid;
            
            Console.WriteLine($"Create Room {guid}");
        }

        public void SendPlayerRoomInfo()
        {
            if (players.Count<=0)
            {
                return;
            }
            var roomInfo = new S2CRoomInfo();
            roomInfo.RoomGuid = guid;
            
            foreach (var pl in players)
            {
                roomInfo.AllPlayers.Add(new C2SLogin(){GId = pl.guid, Name = pl.name});
            }

            foreach (var agent in players)
            {
                var pl = PlayerManager.instance.GetPlayerByGuid(agent.guid);
                pl.SendTCPData(EMessage.S2CRoomInfoRefresh,roomInfo);
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

        public void AddPlayer(RommPlayerInfo pl)
        {
            if (!inMatch)
            {
                players.Add(pl);    
            }
        }

        public void RemovePlayer(int guid)
        {
            for (int i = 0; i < players.Count; i++)
            {
                var pl = players[i];
                if (pl.guid == guid)
                {
                    players.RemoveAt(i);
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
            foreach (var pl in players)
            {
                roomInfo.RoomGuid = guid;
                roomInfo.AllPlayers.Add(new C2SLogin(){GId = pl.guid, Name = pl.name});
            }

            return roomInfo;
        }
    }
}

