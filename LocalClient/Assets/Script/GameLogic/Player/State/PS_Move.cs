using System;
using CenterBase;
using FrameDrive;
using TrueSync;

namespace Game
{
    public class PS_Move:PS_CommomState
    {
        private int speed = 5;
        public PS_Move( FSM<PS_Base> fsm) : base(EPlayerState.Move, fsm){}

        public override void Enter(FSMState<PS_Base> lstState, object param = null)
        {
            base.Enter(lstState, param);
            PlayState("Player_Move");
        }
        
        protected override void LogicUpdate()
        {
            base.LogicUpdate();
            var inputData = FrameInput; 
            if (owner.filed.data.inputData.inputMoveAngle >=0)
            {
                var dir = TSQuaternion.Euler(0, inputData.inputMoveAngle, 0) * TSVector.forward;
                var dt = dir * speed *
                         FrameManager.frameTime;
                var pos = owner.filed.data.pos + dt; 
                pos.x = TSMath.Clamp(pos.x, -5, 5);
                pos.z = TSMath.Clamp(pos.z, -5, 5);
                owner.filed.data.pos = pos;
                owner.filed.data.rot = TSQuaternion.Euler(0, inputData.inputMoveAngle, 0);
            }
            else
            {
                plfsm.SetNextState(EPlayerState.Idle);
            }
        }
    }
}