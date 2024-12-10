using System;
using CenterBase;
using FrameDrive;
using TrueSync;

namespace Game
{
    public class PS_Move:PS_CommomState
    {
        private int speed = 1;
        public PS_Move( FSM<PS_Base> fsm) : base(EPlayerState.Move, fsm){}

        public override void Enter(FSMState<PS_Base> lstState, object param = null)
        {
            base.Enter(lstState, param);
            PlayState("Player_move");
        }
        
        protected override void LogicUpdate()
        {
            base.LogicUpdate();
            var inputData = FrameInput; 
            if (owner.filed.data.inputData.inputMoveAngle >=0)
            {
                var moveDir = TSQuaternion.Euler(0, inputData.inputMoveAngle, 0) * TSVector.forward;
                moveDir *= speed * FrameManager.frameTime;
                owner.filed.data.pos += moveDir;
                if (TSMath.Abs(moveDir.x)>0 )
                {
                    owner.filed.data.faceRight = moveDir.x > 0;
                }
            }
            else
            {
                plfsm.SetNextState(EPlayerState.Idle);
            }
        }
    }
}