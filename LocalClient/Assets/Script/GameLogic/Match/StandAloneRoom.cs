using C2SProtoInterface;
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

        protected override void BeforeEnter()
        {
            base.BeforeEnter();
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
                var pl = new PlayerFiled(new PlayerInfo(){guid = ClientManager.instance.guid,playerName = ClientManager.instance.playerName });
                startInfo[0] = pl;
                for (int i = 1; i <= aiCount; i++)
                {
                    var aiInfo = new PlayerFiled(new PlayerInfo(){guid = i,playerName = $"AI_{i}" });
                    startInfo[i] = aiInfo;
                }

                logicFsm.ChangeState(ELogicType.StandAloneMatching, startInfo);
            }
        }
    }
}