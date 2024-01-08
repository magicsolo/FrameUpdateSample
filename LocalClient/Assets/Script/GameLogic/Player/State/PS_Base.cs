using System.Diagnostics.CodeAnalysis;
using CenterBase;

namespace Game
{
    public enum EPlayerState
    {
        Idle,
        Move,
        Total,
    }

    public class PlayerFSM : FSM<PS_Base>
    {
        public LogicPlayer owner;
        public PlayerFSM(LogicPlayer owner)
        {
            this.owner = owner;
            AddState(new PS_Idle(this));
            AddState(new PS_Move(this));
            ChangeState(EPlayerState.Idle);
        }
        public void ChangeState(EPlayerState stateType)
        {
            ChgST((int)stateType,null);
        }

        public PS_Base GetStateByType(EPlayerState stateType)
        {
            return GetState(((int)stateType));
        }
        
    }
    public abstract class PS_Base:FSMState<PS_Base>
    {
        protected PlayerFSM plfsm;
        public LogicPlayer owner => plfsm.owner;
        public PS_Base(EPlayerState stateType, FSM<PS_Base> fsm) : base((int)stateType, fsm)
        {
            this.plfsm = (PlayerFSM)fsm;
        }
    }
}