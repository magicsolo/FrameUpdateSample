using CenterBase;

namespace Game
{
    public class PS_Idle:PS_Base
    {
        public PS_Idle( FSM<PS_Base> fsm) : base(EPlayerState.Idle, fsm){}

        public override void Update()
        {
            base.Update();
            if (owner.playerData.InputData.inputMoveAngle >-1)
            {
                plfsm.ChangeState(EPlayerState.Move);
            }
        }
    }
}