using FrameDrive;

namespace Game
{
    public static class AttackManager
    {
        public static void AttackEffect(LogicPlayer attacker, LogicPlayer suffer,AttackData data)
        {
            suffer.filed.data.life.Numerator -= data.datamage;
            if (suffer.filed.data.life.Numerator <= 0)
                suffer.fsm.SetNextState(EPlayerState.Death);
            else
                suffer.fsm.SetNextState(EPlayerState.Hurt,data);
        }
    }
}