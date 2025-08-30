using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using C2SProtoInterface;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    public enum GameType
    {
        Login, //登录
        Play, //正常游戏
        PlayBack, //重播
        TraceFrame, //追帧
        Pause, //掉线
    }

    public struct PlayerServData
    {
        //-1:未连接 0:连接中 大于0:已登录
        public int index;
    }

    public enum GameModelType
    {
        StandAlone,
        Local,
        Server,
    }

    public class GameLogic : MonoBehaviour
    {
        
        public PlayerServData playerData = new PlayerServData() { index = -1 };
        public GameModelType gameModelType;

        public LogicFSM fsm { get; private set; }
        
        private void Start()
        {
            fsm = new LogicFSM();
            switch (gameModelType)
            {
                case GameModelType.StandAlone:
                    fsm.ChangeState(ELogicType.StandAloneRoom);
                    break;
                default:
                    fsm.ChangeState(ELogicType.Disconnection,gameModelType);
                    break;
            }
        }

        private void OnDestroy()
        {
            fsm.Exit();
        }

        private void OnGUI()
        {
            fsm.curState.OnGUIUpdate();

            // switch (state)
            // {
            //     case LogicState.Gaming:
            //         ShowGaming();
            //         break;
            // }
        }

        private void ShowLogin()
        {
            GUIStyle btnStyle;

            //TODO 
            btnStyle = GUI.skin.button;
            btnStyle.fontSize = 120;

            if (playerData.index < 0)
            {
                // if (!ClientManager.instance.connected)
                //     GUILayout.Label("已断开连接",btnStyle);
                // else
                //     if (GUILayout.Button("登录",btnStyle))
                //         Login();
            }
            else
            {
                GUILayout.Label("连接中", btnStyle);
            }
        }

        private void ShowGaming()
        {
            // switch (FrameManager.instance.gameType)
            // {
            //     case GameType.Play:
            //
            //
            //         break;
            //     // case GameType.PlayBack:
            //     //     if (GUILayout.Button("开始",btnStyle))
            //     //     {
            //     //         FrameManager.instance.Play();
            //     //     }
            //     //     break;
            //     // case GameType.Pause:
            //     //     if (GUILayout.Button("继续",btnStyle))
            //     //     {
            //     //         FrameManager.instance.Continue();
            //     //     }
            //     //     break;
            // }
        }

        // Start is called before the first frame updat


        // Update is called once per frame
        void Update()
        {
            fsm.curState.Update();
        }

        
    }
}