using C2SProtoInterface;
using CenterBase;
using Script;
using UnityEngine;

namespace Game
{
    public class MatchingState:LogicState
    {
        public MatchingState( LogicFSM fsm) : base(ELogicType.Match, fsm)
        {
        }

        public override void Enter(FSMState<LogicState> lstState, object param = null)
        {
            ClientManager.instance.RegistNoteListener(EMessage.Restart,Reset);
            base.Enter(lstState, param);
            Reset((param as TCPInfo?)?? default);
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

        void Reset(TCPInfo svInfo )
        {
            S2CStartGame s2c = svInfo.ParseMsgData(S2CStartGame.Parser);
            ClientManager.instance.UDPConnect(s2c.Pot);
            FrameManager.instance.ResetPlay(s2c);
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