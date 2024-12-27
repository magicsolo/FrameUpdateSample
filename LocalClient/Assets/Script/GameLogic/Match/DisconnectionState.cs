using C2SProtoInterface;
using CenterBase;
using UnityEngine;

namespace Game
{

    public class DisconnectionState:LogicState
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

        public override void Exit()
        {
            base.Exit();
        }

        public override void Update()
        {
            
        }

        public DisconnectionState( LogicFSM fsm) : base(ELogicType.Disconnection, fsm)
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

            if (GUILayout.Button("链接", btnStyle))
            {
                ClientManager.instance.Connect();
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
                ClientManager.instance.Login(OnLogin);
        }

        void OnLogin(TCPInfo param)
        {
            logicFsm.ChangeState(ELogicType.Lobby);
        }
    }
}