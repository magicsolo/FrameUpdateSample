using C2SProtoInterface;
using CenterBase;
using Script;
using UnityEngine;

namespace Game
{
    public class MatchingState:LogicState
    {
        private S2CStartGame startGameInfo;
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
            ClientManager.instance.UDPConnect(startGameInfo.Pot);
            FrameManager.instance.ResetPlay(startGameInfo);
            ViewModel.instance.ResetPlayers(FrameManager.instance.players);
        }
        

        public override void Update()
        {
            base.Update();
            FrameManager.instance.RequireFrameDatas();
            FrameManager.instance.PlayFrames();
        }
    }
}