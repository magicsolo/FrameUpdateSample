using System.Collections.Generic;
using C2SProtoInterface;
using CenterBase;
using FrameDrive;
using Script;
using UnityEngine;

namespace Game
{
    public struct MatchInfo
    {
        public int pot;
        public int roomGuid;
        public List<PlayerInfo> players;

        public void Init(S2CMatchInfo gameInfo)
        {
            roomGuid = gameInfo.RoomGuid;
            pot = gameInfo.Pot;
            var playersInfo = gameInfo.Players;
            players = new List<PlayerInfo>();
            for (int i = 0; i < playersInfo.Count; i++)
            {
                var plinfo = playersInfo[i];
                var player = new PlayerInfo();
                player.Init(plinfo,i);
                players.Add(player);
            }
        }
    }
    public class MatchingState:LogicState
    {
        private MatchInfo matchInfo => (MatchInfo)param;
        private ServerMatchDrive driver = new ServerMatchDrive();

        public MatchingState( LogicFSM fsm) : base(ELogicType.Match, fsm)
        {
        }

        protected override void BeforeEnter()
        {
            base.BeforeEnter();
            //RegistTCPListener(EMessage.Restart,OnReset);
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            //OnReset((param as TCPInfo?)?? default);
            Reset();
        }

        public override void OnGUIUpdate()
        {
            base.OnGUIUpdate();
            if (GUILayout.Button("结束游戏",btnStyle))
                ClientManager.instance.SendTCPInfo(EMessage.Restart);
            if (GUILayout.Button("保存录像以及帧输出日志",btnStyle))
                ClientManager.instance.SendTCPInfo(EMessage.PrintFrames, new C2SPrintFrames());
            
            GUILayout.Label($"Frame cli:{FrameManager.instance.curClientFrame} serv:{FrameManager.instance.curServerFrame} ri:{FrameManager.instance.clientRuningFrame}",btnStyle);
        }

        public override void Exit()
        {
            base.Exit();
            driver.StopDrive();
            ClientManager.instance.UnRegistNoteListener(EMessage.Restart);
            ClientManager.instance.UnRegistNoteListener(EMessage.PrintFrames);
            ViewModel.instance.Unit();
        }

        // void OnReset(TCPInfo obj)
        // {
        //     param = new MatchInfo();
        //     matchInfo.Init(obj.ParseMsgData(S2CMatchInfo.Parser));
        //     Reset();
        // }
        void Reset( )
        {
            driver.Start(matchInfo);
            ViewModel.instance.Init();
            //ViewModel.instance.ResetPlayers(FrameManager.instance.players);
        }
    }
}