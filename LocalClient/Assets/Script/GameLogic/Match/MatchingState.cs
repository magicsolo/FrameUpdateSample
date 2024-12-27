using C2SProtoInterface;
using CenterBase;
using FrameDrive;
using Script;
using UnityEngine;

namespace Game
{
    public class MatchingState:LogicState
    {
        private S2CStartGame startGameInfo;
        private ServerMatchDrive driver = new ServerMatchDrive();

        public MatchingState( LogicFSM fsm) : base(ELogicType.Match, fsm)
        {
        }

        protected override void BeforeEnter()
        {
            base.BeforeEnter();
            RegistTCPListener(EMessage.Restart,OnReset);
        }

        public  void Enter()
        {
            OnReset((param as TCPInfo?)?? default);
        }

        public override void OnGUIUpdate()
        {
            base.OnGUIUpdate();
            if (GUILayout.Button("重新开始",btnStyle))
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

        void OnReset(TCPInfo tcpInfo)
        {
            startGameInfo = tcpInfo.ParseMsgData(S2CStartGame.Parser);
            Reset();
        }
        void Reset( )
        {
            
            driver.Start(startGameInfo);
            ViewModel.instance.Init();
            //ViewModel.instance.ResetPlayers(FrameManager.instance.players);
        }
    }
}