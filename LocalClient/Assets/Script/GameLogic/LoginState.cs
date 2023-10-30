﻿using C2SProtoInterface;
using CenterBase;
using UnityEngine;

namespace Game
{

    public class LoginState:LogicState
    {
        private bool isConnecting = false;
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
        
        private void Login()
        {
            ClientManager.instance.Login();
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
            
            if (GUILayout.Button("已断开 重连连接", btnStyle))
            {
                    ClientManager.instance.ReConnect();
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

        private void StartGame()
        {
            //ClientManager.instance.SendTCPInfo(EMessage.StartGame,new C2SStartGame(),OnStartGame);
        }
        
        void OnStartGame(TCPInfo data)
        {
            var startGameInfo = data.ParseMsgData(S2CStartGame.Parser);
            FrameManager.instance.ResetPlay(startGameInfo);
        }
        
        
    }
}