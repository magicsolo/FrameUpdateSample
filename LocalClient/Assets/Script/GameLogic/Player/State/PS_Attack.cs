using CenterBase;
using TrueSync;
using UnityEngine.Playables;

namespace Game
{
    public class PS_Attack:PS_Base
    {
        public PS_Attack( FSM<PS_Base> fsm) : base(EPlayerState.Attack, fsm){}
        public override void Enter(FSMState<PS_Base> lstState, object param = null)
        {
            base.Enter(lstState, param);
            PlayState("Player_attack");
        }
    }
}