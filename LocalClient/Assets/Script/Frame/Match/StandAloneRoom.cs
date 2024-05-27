using C2SProtoInterface;
using CenterBase;
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
                S2CStartGame startInfo = new S2CStartGame();
                var pl = new S2CPlayerData();
                pl.Guid = ClientManager.instance.guid;
                pl.Name = ClientManager.instance.playerName;
                startInfo.Players.Add(pl);
                for (int i = 0; i < aiCount; i++)
                {
                    var aiInfo = new S2CPlayerData();
                    aiInfo.Guid = i;
                    aiInfo.Name = $"AI_{i}";
                    startInfo.Players.Add(aiInfo);
                }

                logicFsm.ChangeState(ELogicType.StandAloneMatching, startInfo);
            }
        }
    }
}