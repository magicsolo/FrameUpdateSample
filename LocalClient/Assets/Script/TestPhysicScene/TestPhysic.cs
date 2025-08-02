using System;
using System.Collections;
using System.Collections.Generic;
using FPPhysic;
using Game;
using TrueSync;
using UnityEngine;

public class TestPhysic : MonoBehaviour
{
    // [SerializeField]
    // private GameObject plane;
    //
    // [SerializeField] 
    // private Transform Q;
    // // Start is called before the first frame update
    // void Start()
    // {
    //     FPPlane pl = new FPPlane();
    //     pl.center = plane.transform.position.ToTSVector();
    //     pl.normal = plane.transform.up.ToTSVector();
    //
    //     var p = PointCheckTool.GetPointCast2Plane(pl, Q.transform.position.ToTSVector());
    //     FPDrawTool.instance.SetPoint(p,"p");
    // }

    //[SerializeField]
    // private GameObject plane1;
    // [SerializeField]
    // private GameObject plane2;
    //
    // private void OnDrawGizmos()
    // {
    //     var l1 = new FPLine()
    //         { point = plane1.transform.position.ToTSVector(), direction = plane1.transform.forward.ToTSVector() };
    //     var l2 = new FPLine()
    //         { point = plane2.transform.position.ToTSVector(), direction = plane2.transform.forward.ToTSVector() };
    //
    //     var (q1, q2) = PointCheckTool.GetClosetPointsBetweenLines(l1, l2);
    //     Gizmos.color = Color.white;
    //     Gizmos.DrawLine(q1.ToVector(), q2.ToVector());
    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawLine(plane1.transform.position, plane1.transform.forward + plane1.transform.position);
    //     Gizmos.DrawLine(q1.ToVector(), l1.point.ToVector());
    //     Gizmos.color = Color.cyan;
    //     Gizmos.DrawLine(plane2.transform.position, plane2.transform.forward + plane2.transform.position);
    //     Gizmos.DrawLine(q2.ToVector(), l2.point.ToVector());
    // }


    [SerializeField] 
    private GameObject point;

    [SerializeField] 
    private GameObject cube;

    [SerializeField] 
    private Vector3 halfLength = Vector3.one * 0.5f;
    
    private void OnDrawGizmos()
    {
        if (point == null || cube == null)
        {
            return;
        }
        var obb = new OBB(cube.transform.position.ToTSVector(),cube.transform.rotation.ToTSQuaternion(),halfLength.ToTSVector());
        var p = point.transform.position.ToTSVector();
        var tp= PointCheckTool.GetClosestPointToOBB(obb, p);
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.DrawSphere(tp.ToVector(),0.2f);
        Gizmos.DrawLine(p.ToVector(),tp.ToVector());
        
        Gizmos.matrix = Matrix4x4.TRS(obb.center.ToVector(), obb.rotation.ToQuaternion(), Vector3.one);//Rotate(obb.rotation.ToQuaternion());
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(obb.halfLengths.x.AsFloat() * 2,obb.halfLengths.y.AsFloat() * 2,obb.halfLengths.z.AsFloat()*2));

    }
}
