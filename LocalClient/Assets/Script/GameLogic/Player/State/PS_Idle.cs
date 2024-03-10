using CenterBase;

namespace Game
{
    public abstract class PS_CommomState : PS_Base
    {
        protected PS_CommomState(EPlayerState stateType, FSM<PS_Base> fsm) : base(stateType, fsm){}
        protected override void LogicUpdate()
        {
            base.LogicUpdate();
            var inputData = input;
            if (inputData.input == EInputEnum.fire)
            {
                _nxtStateType = EPlayerState.Attack;
                _finished = true;
            }
        }
    }
    
    
    public class PS_Idle:PS_CommomState
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