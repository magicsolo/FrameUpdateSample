using System.Collections.Generic;
using C2SProtoInterface;
using FrameDrive;
using UnityEngine;

namespace Game
{
    public struct RoomInfo
    {
        public int guid;
        public List<PlayerInfo> players;

        public void Init(S2CRoomInfo roomInfo)
        {
            guid = roomInfo.RoomGuid;
            players = new List<PlayerInfo>();

            for (int i = 0; i < roomInfo.AllPlayers.Count; i++)
            {
                var pl = roomInfo.AllPlayers[i];
                players.Add(new PlayerInfo(){ name = pl.Name, guid = pl.GId,slot = i});
            }
        }
    }
    
    public class Room:LogicState
    {
        private RoomInfo roomInfo;
        private List<PlayerInfo> players => roomInfo.players;
        private int guid => roomInfo.guid;
        public Room( LogicFSM fsm) : base(ELogicType.Room, fsm)
        {
        }

        protected override void BeforeEnter()
        {
            base.BeforeEnter();
            var tcpInfo = (TCPInfo)param;
            SetRoomInfo(tcpInfo);
            RegistTCPListener(EMessage.S2CRoomInfoRefresh,SetRoomInfo);
            RegistTCPListener(EMessage.S2CLeaveRoom,OnLeaveRoom);
        }

        public override void OnGUIUpdate()
        {
            base.OnGUIUpdate();
            GUILayout.BeginHorizontal();
            GUILayout.Label(guid.ToString());
            if (GUILayout.Button("退出"))
            {
                ClientManager.instance.SendTCPInfo(EMessage.C2SLeaveRoom);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            
            foreach (var pl in players)
            {
                GUILayout.Label($"{pl.name} " );
            }
            GUILayout.EndHorizontal();
        }

        void SetRoomInfo(TCPInfo tcpInfo)
        {
            roomInfo = new RoomInfo();
            roomInfo.Init(tcpInfo.ParseMsgData(S2CRoomInfo.Parser));
        }
        
        private void OnLeaveRoom(TCPInfo obj)
        {
            logicFsm.ChangeState(ELogicType.Lobby);
        }
    }
}