﻿using C2SProtoInterface;
using CenterBase;
using FrameDrive;
using UnityEngine;

namespace Game
{
    public class StandAloneRoom:LogicState
    {
        
        public int aiCount;
        public StandAloneRoom( LogicFSM fsm) : base(ELogicType.StandAloneRoom, fsm)
        {
        }

        public override void Enter(FSMState<LogicState> lstState, object param = null)
        {
            base.Enter(lstState, param);
            aiCount = 0;
        }

        public override void OnGUIUpdate()
        {
            base.OnGUIUpdate();
            GUILayout.BeginHorizontal();
            GUILayout.Label($"ai数量{aiCount}",txtStyle);
            if (GUILayout.Button("+",btnStyle))
                ++aiCount;
            if (aiCount > 0 && GUILayout.Button("-",btnStyle))
                --aiCount;
            GUILayout.EndHorizontal();
            if (GUILayout.Button("开始测试",btnStyle))
            {
                PlayerFiled[] startInfo = new PlayerFiled[1 + aiCount];
                var pl = new PlayerFiled();
                pl.info.guid = ClientManager.instance.guid;
                pl.info.name = ClientManager.instance.playerName;
                startInfo[0] = pl;
                for (int i = 1; i <= aiCount; i++)
                {
                    var aiInfo = new PlayerFiled();
                    aiInfo.info.guid = i;
                    aiInfo.info.name = $"AI_{i}";
                    startInfo[i] = aiInfo;
                }

                logicFsm.ChangeState(ELogicType.StandAloneMatching, startInfo);
            }
        }
    }
}