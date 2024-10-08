﻿using CenterBase;
using FrameDrive;
using TrueSync;
using UnityEngine;
using UnityEngine.Playables;

namespace Game
{
    public struct HurtInfo
    {
        public TSVector dir;
        public LogicPlayer attacker;
        public LogicPlayer suffer;
    }
    public class PS_Attack:PS_Base
    {
        public PS_Attack( FSM<PS_Base> fsm) : base(EPlayerState.Attack, fsm){}
        public override void Enter(FSMState<PS_Base> lstState, object param = null)
        {
            base.Enter(lstState, param);
            PlayState("Player_attack");
        }

        public override void OnFire()
        {
            base.OnFire();
            var pos = owner.filed.data.pos;
            pos.x += _curSkillPos.x * (owner.filed.data.faceRight ? 1 : -1);
            pos.y += _curSkillPos.y;
            var area = _curSkillArea;

            TSVector plMin = pos;
            TSVector plMax = pos;

            plMin.x -= area.x/2;
            plMax.x += area.x / 2;
            plMin.y -= area.y/2;
            plMax.y += area.y / 2;
            plMin.z -= area.z/2;
            plMax.z += area.z / 2;
            foreach (var target in owner.match.allPlayers)
            {
                if (target.filed.info.guid != owner.filed.info.guid)
                {
                    var tarMin = target.filed.data.pos;
                    tarMin.y += FP.One*2/10;
                    var tarMax = tarMin;
                    FP width = FP.One * 48 / 100;
                    FP height = FP.One * 44 / 100;

                    tarMin.x -= width / 2;
                    tarMax.x += width / 2;
                    tarMin.y -= height / 2;
                    tarMax.y += height / 2;

                    bool collide = IsCubeCollide(plMin, plMax, tarMin, tarMax);
                    if (collide)
                    {
                        var suffer = new HurtInfo()
                            {attacker = owner,suffer = target,dir = owner.filed.data.faceRight? TSVector.right : TSVector.left};
                        target.fsm.SetNextState(EPlayerState.Hurt,suffer);
                    }
                }
            }
            
        }

        bool IsCubeCollide(TSVector c1Min, TSVector c1Max, TSVector c2Min, TSVector c2Max)
        {
            //相交
            if ((TSMath.Max(c1Min.x, c2Min.x) <= TSMath.Min(c1Max.x, c2Max.x))
                &&(TSMath.Max(c1Min.y,c2Min.y)<=TSMath.Min(c1Max.y,c2Max.y))
                &&(TSMath.Max(c1Min.z,c2Min.z)<=TSMath.Min(c1Max.z,c2Max.z)))
            {
                return true;
            }

            //分离
            return false;
        }
    }
}