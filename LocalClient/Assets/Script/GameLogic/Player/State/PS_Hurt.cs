using CenterBase;

namespace Game
{
    public class PS_Hurt: PS_Base
    {
        public PS_Hurt(FSM<PS_Base> fsm) : base(EPlayerState.Hurt, fsm)
        {
            
        }

        public override void Enter(FSMState<PS_Base> lstState, object param = null)
        {
            base.Enter(lstState, param);
            var data = (HurtInfo)param;
            owner.playerData.faceRight = data.dir.x < 0;
            PlayState("Player_hurt");
        }
    }
}