using CenterBase;

namespace Game
{
    public class PS_Death:PS_Base
    {
        public PS_Death( FSM<PS_Base> fsm) : base(EPlayerState.Death, fsm)
        {
            useAnimDrive = true;
            isLoop = true;
        }

        public override void Enter(FSMState<PS_Base> lstState, object param = null)
        {
            base.Enter(lstState, param);
            PlayState("Player_Death");
        }
    }
}