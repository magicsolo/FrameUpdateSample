using Game;
using TrueSync;

namespace FPPhysic
{
    public static class PointCheckTool
    {
        //点到平面的投影 看起来有个更好的方法可以得到，比如将点转换到平面的坐标系中
        public static TSVector GetPointCast2Plane(FPPlane plane, TSVector point)
        {

            var p2p = point - plane.center;
            
            return point - (plane.normal * p2p) / (plane.normal * plane.normal) * plane.normal;
        }

        public static TSVector GetClosestPointToOBB(OBB obb, TSVector point)
        {
            TSVector d = point - obb.center;
            TSVector tarP = obb.center;
            var disX = obb.axX * d;
            tarP += obb.axX * (TSMath.Sign(disX)*TSMath.Min(obb.halfLengths.x,TSMath.Abs(disX)));
            var disY = obb.axY * d;
            tarP += obb.axY * (TSMath.Sign(disY) * TSMath.Min(obb.halfLengths.y, TSMath.Abs(disY)));
            var disZ = obb.axZ * d;
            tarP += obb.axZ * (TSMath.Sign(disZ) * TSMath.Min(obb.halfLengths.z, TSMath.Abs(disZ)));
            return tarP;
        }

        //获取两直线的最近点
        public static (TSVector, TSVector) GetClosetPointsBetweenLines(FPLine line1, FPLine line2)
        {
            var r = line1.point - line2.point;
            var a = TSVector.Dot(line1.direction, line1.direction);
            var b = -TSVector.Dot(line1.direction, line2.direction);
            var c = TSVector.Dot(line2.direction, line1.direction);
            var d = -TSVector.Dot(line2.direction, line2.direction);
            var e = -TSVector.Dot(line1.direction, r);
            var f = -TSVector.Dot(line2.direction, r);

            var q1 = line1.point + (d * e - b * f) / (a * d - b * c) * line1.direction;
            var q2 = line2.point + (a * f - c * e) / (a * d - b * c) * line2.direction;
            return (q1, q2);
        }
        
    }
}