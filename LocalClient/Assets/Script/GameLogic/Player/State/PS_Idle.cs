﻿using CenterBase;

namespace Game
{
    public class PS_Idle:PS_Base
    {
        public PS_Idle( FSM<PS_Base> fsm) : base(EPlayerState.Idle, fsm){}

        public override void Enter(FSMState<PS_Base> lstState, object param = null)
        {
            base.Enter(lstState, param);
            PlayState("Player_idle");
        }

        protected override void LogicUpdate()
        {
            base.LogicUpdate();
            if (owner.playerData.InputData.inputMoveAngle >-1)
            {
                plfsm.ChangeState(EPlayerState.Move);
            }
        }
    }
}