
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

            var newRoom = GenerateNewRoom();
            newRoom.AddPlayer(agent.guid);
            registPlayers[agent.guid] = newRoom;
            newRoom.SendPlayerRoomInfo();
        }
        
        
        GameRoomAgent GenerateNewRoom()
        {
            var newRoom = new GameRoomAgent(++guidIndex);
            lock (gameRoomAgents)
            {
                gameRoomAgents.Add(newRoom.guid,newRoom);
            }
            return newRoom;
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

        public void RemoveRoom(int roomId)
        {
            lock (gameRoomAgents)
            {
                gameRoomAgents.Remove(roomId);
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


        public GameRoomAgent GetRoomAgent(int roomGuid)
        {
            if (gameRoomAgents.TryGetValue(roomGuid,out var room))
            {
                return room;
            }

            return null;
        }
        public GameRoomAgent GetRoomAgentByPlayerGuid(int guid)
        {
            if (registPlayers.TryGetValue(guid, out var roomAgent))
            {
                return roomAgent;
            }
            return default;
        }

        public void ReqAllRoomInfo(PlayerAgent agent)
        {
            var allRommInfo = new S2CAllRoomInfo();
            foreach (var kv in gameRoomAgents)
                allRommInfo.AllRooms.Add(kv.Value.GetAllRoomInfo());

            agent.SendTCPData(EMessage.C2SReqAllRoomInfos, allRommInfo);
        }

        public void ReqStartMatch(PlayerAgent agent)
        {
            if (registPlayers.TryGetValue(agent.guid,out var roomAgent))
            {
                roomAgent.StartMatch();
            }
        }

        public void FromMatchBackToRoom(PlayerAgent player)
        {
            var room = GetRoomAgentByPlayerGuid(player.guid);
            var roomInfo = room.GetRoomInfo();
            foreach (var playerInfo in roomInfo.AllPlayers)
            {
                room.SendPlayerTCP(playerInfo.Guid, EMessage.S2CEndMatch, roomInfo);
            }
        }
        
        // public void UpdateRooms()
        // {
        //     lock (gameRoomAgents)
        //     {
        //         foreach (var room in gameRoomAgents.Values)
        //         {
        //             room.Update();
        //         }
        //     }
        // }
    }    
}
