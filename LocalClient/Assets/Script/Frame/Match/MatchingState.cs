using C2SProtoInterface;
using CenterBase;
using Script;
using UnityEngine;

namespace Game
{
    public class MatchingState:LogicState
    {
        private S2CStartGame startGameInfo;
        private LogicDrive driver = new LogicDrive();

        public MatchingState( LogicFSM fsm) : base(ELogicType.Match, fsm)
        {
        }

        public override void Enter(FSMState<LogicState> lstState, object param = null)
        {
            ClientManager.instance.RegistNoteListener(EMessage.Restart,OnReset);
            base.Enter(lstState, param);
            OnReset((param as TCPInfo?)?? default);
        }

        public override void OnGUIUpdate()
        {
            base.OnGUIUpdate();
            if (GUILayout.Button("重新开始",btnStyle))
                ClientManager.instance.SendTCPInfo(EMessage.Restart);
            if (GUILayout.Button("打印帧",btnStyle))
                ClientManager.instance.SendTCPInfo(EMessage.PrintFrames, new C2SPrintFrames(), OnPrintFrames);
            
            GUILayout.Label($"Frame cli:{FrameManager.instance.curClientFrame} serv:{FrameManager.instance.curServerFrame}",btnStyle);
        }

        private void OnPrintFrames(TCPInfo obj)
        {
            FrameManager.instance.OnPrintFrames();
        }

        public override void Exit()
        {
            base.Exit();
            ClientManager.instance.UnRegistNoteListener(EMessage.Restart);
        }

        void OnReset(TCPInfo tcpInfo)
        {
            startGameInfo = tcpInfo.ParseMsgData(S2CStartGame.Parser);
            Reset();
        }
        void Reset( )
        {
            driver.Start(startGameInfo);
            ViewModel.instance.Init(startGameInfo,driver.match);
            //ViewModel.instance.ResetPlayers(FrameManager.instance.players);
        }
    }
}