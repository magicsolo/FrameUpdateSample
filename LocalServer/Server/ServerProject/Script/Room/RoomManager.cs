
using C2SProtoInterface;

namespace GameServer
{
    public class RoomManager:Single<RoomManager>
    {
        private int guidIndex = 0;
        private Dictionary<int, GameRoomAgent> gameRoomAgents = new Dictionary<int, GameRoomAgent>(); 
        private Dictionary<int, GameRoomAgent> registPlayers = new Dictionary<int, GameRoomAgent>();

        public void ReqCreateRoom(PlayerAgent agent)
        {
            if (registPlayers.TryGetValue(agent.guid,out var room))
            {
                room.SendPlayerRoomInfo();
                return;
            }

            var newRoom = new GameRoomAgent(agent.guid,++guidIndex);
            lock (gameRoomAgents)
            {
                gameRoomAgents.Add(newRoom.guid,newRoom);
            }
            registPlayers[agent.guid] = newRoom;
            newRoom.SendPlayerRoomInfo();
        }

        public void ReqLeaveRoom(PlayerAgent agent)
        {
            if (registPlayers.TryGetValue(agent.guid,out var roomAgent))
            {
                roomAgent.OnReqLeaveRoom(agent);
                registPlayers.Remove(agent.guid);
                if (roomAgent.playerCount <= 0)
                {
                    lock (gameRoomAgents)
                    {
                        gameRoomAgents.Remove(roomAgent.guid);
                    }
                }
            }
        }

        public void ReqJoinRoom(PlayerAgent agent,byte[] streamBuffer,int len)
        {
            int offset = sizeof(EMessage);
            var data = C2SJoinRoom.Parser.ParseFrom(streamBuffer, offset, len - offset);
            if (gameRoomAgents.TryGetValue(data.RoomGuid,out var roomAgent))
            {
                roomAgent.OnReqJoinRoom(agent);
                if (!registPlayers.ContainsKey(agent.guid))
                {
                    registPlayers.Add(agent.guid,roomAgent);
                }
            }
        }

        public GameRoomAgent GetRoomAgentByPlayerGuid(int guid)
        {
            if (registPlayers.TryGetValue(guid, out var roomAgent))
            {
                return roomAgent;
            }
            return default;
        }

        public void ReqRoomInfo(PlayerAgent agent)
        {
            var allRommInfo = new S2CAllRoomInfo();
            foreach (var kv in gameRoomAgents)
                if(!kv.Value.inMatch)
                    allRommInfo.AllRooms.Add(kv.Value.GetAllRoomInfo());

            agent.SendTCPData(EMessage.ReqRoomInfos, allRommInfo);
        }

        public void ReqStartMatch(PlayerAgent agent)
        {
            if (registPlayers.TryGetValue(agent.guid,out var roomAgent))
            {
                roomAgent.StartMatch();
            }
        }

        public void ReqEndMatch(PlayerAgent agent)
        {
            if (registPlayers.TryGetValue(agent.guid,out var roomAgent))
            {
                roomAgent.EndMatch();
            }
        }

        public void ReceiveUDPData(int guid,C2SFrameUpdate data)
        {
            if (registPlayers.TryGetValue(guid,out var roomAgent))
            {
                roomAgent.ReceiveC2SFrameData(guid, data);
            }
        }

        public void UpdateRooms()
        {
            lock (gameRoomAgents)
            {
                foreach (var room in gameRoomAgents.Values)
                {
                    room.Update();
                }
            }
            
        }
    }    
}
