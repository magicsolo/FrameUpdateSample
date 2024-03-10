using System;
using CenterBase;
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
            var inputData = input; 
            if (owner.playerData.InputData.inputMoveAngle >-1)
            {
                var moveDir = TSQuaternion.Euler(0, inputData.inputMoveAngle, 0) * TSVector.forward;

                moveDir *= speed * FrameManager.instance.frameTime;
                owner.playerData.pos += moveDir;
                if (TSMath.Abs(moveDir.x)>0 )
                {
                    owner.playerData.faceRight = moveDir.x > 0;
                }
            }
            else
            {
                plfsm.ChangeState(EPlayerState.Idle);
            }
        }
    }
}