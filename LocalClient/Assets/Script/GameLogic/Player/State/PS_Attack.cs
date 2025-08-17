using CenterBase;
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

    public struct OBB
    {
        public TSVector center;
        public TSVector axX => rotation * TSVector.right;
        public TSVector axY => rotation * TSVector.up;
        public TSVector axZ => rotation * TSVector.forward;

        public TSQuaternion rotation { get;private set; }
        public TSVector halfLengths;

        public OBB(TSVector center,TSQuaternion rotation, TSVector halfLengths)
        {
            this.center = center;
            this.rotation = rotation;
            this.halfLengths = halfLengths;
        }
        
        public void SetRotate(TSQuaternion rot)
        {
            this.rotation = rot;
        }

        public TSVector GetAxis(int idx)
        {
            switch (idx)
            {
                case 0: return axX;
                case 1: return axY;
                default:; return axZ;
            }
        }

        public FP GetHalfLength(int idx)
        {
            switch (idx)
            {
                case 0: return halfLengths.x;
                case 1: return halfLengths.y;
                default: return halfLengths.z;
            }
        }
    }
    public class PS_Attack:PS_Base
    {
        public PS_Attack( FSM<PS_Base> fsm) : base(EPlayerState.Attack, fsm){}
        public override void Enter(FSMState<PS_Base> lstState, object param = null)
        {
            base.Enter(lstState, param);
            PlayState("Player_Attack");
        }

        public override void OnFire()
        {
            base.OnFire();
            var pos = owner.filed.data.pos;
            var dt = owner.filed.data.rot * TSVector.forward * _curSkillPos.x;
            dt.y += _curSkillPos.y;
            pos += dt;

            var selfOBB = GetOBBB(pos, _curSkillArea);
            selfOBB.SetRotate(owner.filed.data.rot);

            foreach (var target in owner.match.allPlayers)
            {
                if (target.filed.info.guid != owner.filed.info.guid)
                {
                    var tarMin = target.filed.data.pos;
                    
                    tarMin.y += FP.One;
                    var tarMax = tarMin;
                    FP width = FP.One * 48 / 100;
                    FP height = FP.One * 44 / 100;

                    tarMin.x -= width / 2;
                    tarMax.x += width / 2;
                    tarMin.y -= height / 2;
                    tarMax.y += height / 2;
                    
                    var targetOBB = GetOBBB(target.filed.data.pos+TSVector.up, new TSVector(width, height,width));
                    targetOBB.SetRotate(target.filed.data.rot);
                    if (CheckCollision(selfOBB,targetOBB))
                    {
                        target.fsm.SetNextState(EPlayerState.Hurt,new AttackData(){ attacker = target, datamage = 1});
                    }
                }
            }
            
        }

        OBB GetOBBB(TSVector pos,TSVector area)
        {
            return new OBB(){ center = pos, halfLengths = new TSVector() { x = area.x / 2, y = area.y / 2, z = area.z / 2 } };
        }
        TSVector[] axesToTest = new TSVector[15];
        public bool CheckCollision(OBB a, OBB b)
        {
            // 分离轴集合：15条轴（两个OBB的3个主轴 + 3x3的叉乘轴）
            // 添加第一个OBB的3个轴
            axesToTest[0] = a.axX;//xis[0];
            axesToTest[1] = a.axY;//[1];
            axesToTest[2] = a.axZ;//[2];
        
            // 添加第二个OBB的3个轴
            axesToTest[3] = b.axX;
            axesToTest[4] = b.axY;
            axesToTest[5] = b.axZ;
        
            // 计算两个OBB各轴的叉乘轴（共9条）
            int index = 6;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    axesToTest[index] = TSVector.Cross(axesToTest[i], axesToTest[3+j]);
                    // 叉乘结果如果是零向量，跳过（平行轴无需检测）
                    if ( axesToTest[index].sqrMagnitude< (FP.One / 10000))
                        continue;
                    axesToTest[index] = axesToTest[index].normalized;
                    index++;
                }
            }

            for (int i = 6; i < axesToTest.Length; i++)
            {
                var axis = axesToTest[i];
                
                if (axis == TSVector.zero) continue; // 忽略无效轴

                // 计算两个OBB在该轴上的投影半长
                FP aProjection = GetProjectionRadius(a, axis);
                FP bProjection = GetProjectionRadius(b, axis);
            
                // 计算两个中心点在该轴上的投影距离
                TSVector centerDelta = b.center - a.center;
                FP distance = TSMath.Abs(TSVector.Dot(centerDelta, axis));

                // 如果投影距离大于两半长之和，说明在此轴上分离，未碰撞
                if (distance > aProjection + bProjection)
                {
                    return false;
                }
            }

            // 所有轴均未分离，说明碰撞
            return true;
        }
        
        FP GetProjectionRadius(OBB obb, TSVector axis)
        {
            FP projection = 0;
            for (int i = 0; i < 3; i++)
            {
                // 计算每个轴的贡献：|轴i · 分离轴| * 半长
                projection += TSMath.Abs(TSVector.Dot(obb.GetAxis(i), axis)) * obb.GetHalfLength(i);
            }
            return projection;
        }
    }
}