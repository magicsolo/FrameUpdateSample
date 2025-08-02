using CenterBase;
using FrameDrive;
using TrueSync;

namespace Game
{
    public class PS_Hurt: PS_Base
    {

        public PS_Hurt(FSM<PS_Base> fsm) : base(EPlayerState.Hurt, fsm)
        {
            useAnimDrive = true;
        }

        public override void Enter(FSMState<PS_Base> lstState, object param = null)
        {
            var attacker = (LogicPlayer)param;
            owner.filed.data.rot = TSQuaternion.FromToRotation(TSVector.forward,attacker.filed.data.pos - owner.filed.data.pos);
            base.Enter(lstState, param);
            
            PlayState("Player_Hurt");
        }
    }
}