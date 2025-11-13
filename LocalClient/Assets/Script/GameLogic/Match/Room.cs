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
                players.Add(new PlayerInfo(){ playerName = pl.Name, guid = pl.Guid,slot = i});
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
            SetRoomInfo((S2CRoomInfo)param);
            RegistTCPListener(EMessage.S2CRoomInfoRefresh,OnGetRoomInfo);
            RegistTCPListener(EMessage.S2CLeaveRoom,OnLeaveRoom);
            RegistTCPListener(EMessage.S2CStartMatch,OnEnterGame);
        }


        public override void OnGUIUpdate()
        {
            base.OnGUIUpdate();
            GUILayout.BeginHorizontal();
            GUILayout.Label(guid.ToString());
            if (players[0].guid == ClientManager.instance.guid && GUILayout.Button("开始"))
            {
                ClientManager.instance.SendTCPInfo(EMessage.C2SStartMatch);
            }
            if (GUILayout.Button("退出"))
            {
                ClientManager.instance.SendTCPInfo(EMessage.C2SLeaveRoom);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            
            foreach (var pl in players)
            {
                GUILayout.Label($"{pl.playerName} " );
            }
            GUILayout.EndHorizontal();
        }

        void OnGetRoomInfo(TCPInfo tcpInfo)
        {
            
            SetRoomInfo(tcpInfo.ParseMsgData(S2CRoomInfo.Parser));
        }

        void SetRoomInfo(S2CRoomInfo s2cRoomInfo)
        {
            roomInfo = new RoomInfo();
            roomInfo.Init(s2cRoomInfo);
        }
        
        private void OnLeaveRoom(TCPInfo obj)
        {
            logicFsm.ChangeState(ELogicType.Lobby,obj.ParseMsgData(S2CAllRoomInfo.Parser));
        }
        
        
        private void OnEnterGame(TCPInfo obj)
        {
            var MatchInfo = new MatchInfo(obj.ParseMsgData(S2CMatchInfo.Parser));
            logicFsm.ChangeState(ELogicType.Match,MatchInfo);
        }
    }
}