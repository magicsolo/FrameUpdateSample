using CenterBase;
using TrueSync;

namespace Game
{
    public class PS_Move:PS_Base
    {
        private int speed = 1;
        public PS_Move( FSM<PS_Base> fsm) : base(EPlayerState.Move, fsm){}
        public override void Update()
        {
            base.Update();
            var inputData = owner.playerData.InputData; 
            if (owner.playerData.InputData.inputMoveAngle >-1)
            {
                var moveDir = TSQuaternion.Euler(0, inputData.inputMoveAngle, 0) * TSVector.forward;

                moveDir *= speed * FrameManager.instance.frameTime;
                owner.playerData.pos += moveDir;
            }
            else
            {
                plfsm.ChangeState(EPlayerState.Idle);
            }
        }
    }
}