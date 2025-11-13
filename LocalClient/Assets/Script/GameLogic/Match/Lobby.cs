using System.Collections.Generic;
using C2SProtoInterface;
using FrameDrive;
using UnityEngine;

namespace Game
{
    public class Lobby:LogicState
    {
        Vector2 scrollPos;
        List<RoomInfo> allRooms = new List<RoomInfo>();
        public Lobby( LogicFSM fsm) : base(ELogicType.Lobby, fsm)
        {
        }

        protected override void BeforeEnter()
        {
            base.BeforeEnter();
            RegistEvent(EventKeys.OnOffLine,OnBeLogOut);
            RegistTCPListener(EMessage.S2CRoomInfoRefresh,OnEnterRoom);
            RegistTCPListener(EMessage.S2CStartMatch,OnEnterGame);
        }

        protected override void AfterEnter()
        {
            base.AfterEnter();
            SetViewRoomInfos((S2CAllRoomInfo)param);
        }
        
        public override void OnGUIUpdate()
        {
            base.OnGUIUpdate();
            GUILayout.BeginHorizontal();
            GUILayout.Label(ClientManager.instance.playerName, txtStyle);
            if (GUILayout.Button("登出",btnStyle))
            {
                ClientManager.instance.LogOut();
            }

            if (GUILayout.Button("刷新当前房间列表"))
            {
                ClientManager.instance.SendTCPInfo(EMessage.C2SReqAllRoomInfos,callBack:OnGetRoomInfos);
            }

            if (GUILayout.Button("创建房间"))
            {
                ClientManager.instance.SendTCPInfo(EMessage.C2SCreateRoom);
            }
            GUILayout.EndHorizontal();
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            foreach (RoomInfo room in allRooms)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label($"{room.guid} {room.players[0].playerName}");
                if (GUILayout.Button("进入"))
                {
                    EnterRoom(room.guid);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            
        }

        void EnterRoom(int roomGuid)
        {
            ClientManager.instance.SendTCPInfo(EMessage.C2SJoinRoom,new C2SJoinRoom(){RoomGuid = roomGuid});
        }

        private void OnBeLogOut(object param)
        {
            logicFsm.ChangeState(ELogicType.Disconnection);
        }

        private void OnEnterRoom(TCPInfo tcpInfo)
        {
            logicFsm.ChangeState(ELogicType.Room,tcpInfo.ParseMsgData(S2CRoomInfo.Parser));
        }

        private void OnEnterGame(TCPInfo tcpInfo)
        {
            var MatchInfo = new MatchInfo(tcpInfo.ParseMsgData(S2CMatchInfo.Parser));
            logicFsm.ChangeState(ELogicType.Match,MatchInfo);
        }
        
        void OnGetRoomInfos(TCPInfo tcpInfo)
        {
            allRooms.Clear();
            var roomInfo = tcpInfo.ParseMsgData(S2CAllRoomInfo.Parser);
            SetViewRoomInfos(roomInfo);
        }

        void SetViewRoomInfos(S2CAllRoomInfo roomInfo)
        {
            foreach (var room in roomInfo.AllRooms)
            {
                var newRoom = new RoomInfo();
                newRoom.Init(room);
                allRooms.Add(newRoom);
            }
        }

    }
}