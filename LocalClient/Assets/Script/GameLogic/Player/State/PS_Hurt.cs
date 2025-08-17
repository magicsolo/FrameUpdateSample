using System;
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
            var data = (AttackData)param;
            var attacker = data.attacker;
            var damage = data.datamage;
            owner.filed.data.life.Numerator = Math.Max(attacker.filed.data.life.Numerator - damage,0);
            owner.filed.data.rot = TSQuaternion.FromToRotation(TSVector.forward,attacker.filed.data.pos - owner.filed.data.pos);
            base.Enter(lstState, param);
            
            PlayState("Player_Hurt");
        }
    }
}