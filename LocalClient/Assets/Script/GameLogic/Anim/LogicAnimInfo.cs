using System;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    [Serializable]
    public struct LogicFrameCurveInfo
    {
        public FP inTan;
        public FP outTan;
        public int frame;
        public FP Value;

    }
    
    [Serializable]
    public struct LogicAnimInfo
    {
        public string key;
        public string controllerName;
        public int length;
        public string stateName;
        public int fireFrame;
        
        public LogicFrameCurveInfo[] skillAreaX;
        public LogicFrameCurveInfo[] skillAreaY;
        public LogicFrameCurveInfo[] skillAreaZ;

        public LogicFrameCurveInfo[] skillPosX;
        public LogicFrameCurveInfo[] skillPosY;
        public LogicFrameCurveInfo[] skillPosZ;

        public LogicFrameCurveInfo[] posX;
        public LogicFrameCurveInfo[] posY;
        public LogicFrameCurveInfo[] posZ;

        public TSVector GetSkillArea(int frame)
        {
            var vect = TSVector.zero;
            vect.x = GetCurveValue(skillAreaX, frame);
            vect.y = GetCurveValue(skillAreaY, frame);
            vect.z = GetCurveValue(skillPosZ, frame);

            return vect;
        }

        public TSVector2 GetSkillPos(int frame)
        {
            var vect = TSVector2.zero;
            vect.x = GetCurveValue(skillPosX, frame);
            vect.y = GetCurveValue(skillPosY, frame);
            return vect;
        }

        public TSVector GetPos(int frame)
        {
            var vect = TSVector.zero;
            vect.x = GetCurveValue(posX, frame);
            vect.y = GetCurveValue(posY, frame);
            vect.z = GetCurveValue(posZ, frame);
            return vect;
        }

        FP GetCurveValue(LogicFrameCurveInfo[] arr, int frame)
        {
            var (lst, nxt) = getCurveInfo(arr, frame);
            int leng = nxt.frame - lst.frame;
            if (leng == 0)
                return lst.Value;

            FP t = frame / leng;
            FP t2 = TSMath.Pow(t, 2);
            FP t3 = TSMath.Pow(t, 3);
            FP p0 = lst.Value;
            FP p1 = nxt.Value;
            FP m0 = lst.outTan * leng;
            FP m1 = nxt.inTan * leng; 
            //P(t) =(2*t^3-3*t^2+1)*P_0+(t^3-2*t^2+t)*M_0+(t^3-t^2)*M_1+(-2*t^3+3*t^2)*P_1
            FP v = (2 * t3 - 3 * t2 + FP.One)*p0+(t3-2*t2+t)*m0+(t3-t2)*m1+(-2*t3+3*t2)*p1;
            return v;
        }
        
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="frame"></param>
        /// <returns>上一帧，下一帧</returns>
        (LogicFrameCurveInfo, LogicFrameCurveInfo) getCurveInfo(LogicFrameCurveInfo[] arr, int frame)
        {
            if (arr == null || arr.Length == 0)
            {
                return default;
            }

            LogicFrameCurveInfo lst = arr[0];
            LogicFrameCurveInfo nxt = arr[0];
            for (int idx = 1; idx < arr.Length; idx++)
            {
                if (nxt.frame >= frame)
                {
                    break;
                }
                lst = nxt;
                nxt = arr[idx];
            }
            return (lst, nxt);
        }
        
    }

}