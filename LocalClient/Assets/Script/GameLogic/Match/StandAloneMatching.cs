using C2SProtoInterface;
using CenterBase;
using FrameDrive;
using Script;
using UnityEngine;

namespace Game
{
    public class StandAloneMatching:LogicState
    {
        private StandAloneMatchDrive driver = new StandAloneMatchDrive();
        public StandAloneMatching( LogicFSM fsm) : base(ELogicType.StandAloneMatching, fsm)
        {
        }

        public override void Enter(FSMState<LogicState> lstState, object param = null)
        {
            base.Enter(lstState, param);
            driver.Start((PlayerFiled[]) param);
            ViewModel.instance.Init();
        }

        public override void OnGUIUpdate()
        {
            base.OnGUIUpdate();
            if (GUILayout.Button("Leave"))
            {
                logicFsm.ChangeState(ELogicType.StandAloneRoom);
            }
        }

        public override void Update()
        {
            base.Update();
            driver.Update();
        }

        public override void Exit()
        {
            base.Exit();
            driver.Stop();
            ViewModel.instance.Unit();
        }
    }
}