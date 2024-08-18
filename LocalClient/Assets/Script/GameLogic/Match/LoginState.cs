using C2SProtoInterface;
using CenterBase;
using UnityEngine;

namespace Game
{

    public class LoginState:LogicState
    {
        private bool isConnecting = false;
        private string videoName = "log";
        public override void OnGUIUpdate()
        {
            base.OnGUIUpdate();
            switch (ClientManager.instance.tcpState)
            {
                case EConnecterState.Connecting:
                    GUILayout.Label("连接中",btnStyle);
                    break;
                case EConnecterState.DisConnect:
                    ShowDisConnected();
                    break;
                case EConnecterState.Connected:
                    ShowLog();
                    break;
            }
        }

        public override void Enter(FSMState<LogicState> lstState, object param = null)
        {
            base.Enter(lstState, param);
            ClientManager.instance.RegistNoteListener(EMessage.Restart,OnLogin);
        }

        public override void Exit()
        {
            base.Exit();
            ClientManager.instance.UnRegistNoteListener(EMessage.Restart);
        }

        public override void Update()
        {
            
        }

        public LoginState( LogicFSM fsm) : base(ELogicType.Login, fsm)
        {
        }

        void ShowDisConnected()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("ip:", btnStyle);
            ClientManager.instance.ip = GUILayout.TextArea(ClientManager.instance.ip, btnStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Pot:", btnStyle);
            ClientManager.instance.pot = GUILayout.TextArea(ClientManager.instance.pot, btnStyle);
            GUILayout.EndHorizontal();

            if (GUILayout.Button("已断开", btnStyle))
            {
                ClientManager.instance.ReConnect();
            }
            else
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("看录像",btnStyle))
                {
                    logicFsm.ChangeState(ELogicType.Video,videoName);
                }
                videoName = GUILayout.TextArea(videoName, btnStyle);
                GUILayout.EndHorizontal();
            }
        }
        
        void ShowLog()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("名字:", btnStyle);
            ClientManager.instance.playerName = GUILayout.TextArea(ClientManager.instance.playerName, btnStyle);
            GUILayout.EndHorizontal();
            
            if (GUILayout.Button("登录",btnStyle))
                ClientManager.instance.Login();
        }
        void OnLogin(TCPInfo servDat)
        {
            logicFsm.ChangeState(ELogicType.Match,servDat);
        }
    }
}