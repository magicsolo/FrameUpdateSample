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

    [SerializeField]
    private GameObject plane1;
    [SerializeField]
    private GameObject plane2;

    private void OnDrawGizmos()
    {
        var l1 = new FPLine()
            { point = plane1.transform.position.ToTSVector(), direction = plane1.transform.forward.ToTSVector() };
        var l2 = new FPLine()
            { point = plane2.transform.position.ToTSVector(), direction = plane2.transform.forward.ToTSVector() };

        var (q1, q2) = PointCheckTool.GetClosetPointsBetweenLines(l1, l2);
        Gizmos.color = Color.white;
        Gizmos.DrawLine(q1.ToVector(), q2.ToVector());
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(plane1.transform.position, plane1.transform.forward + plane1.transform.position);
        Gizmos.DrawLine(q1.ToVector(), l1.point.ToVector());
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(plane2.transform.position, plane2.transform.forward + plane2.transform.position);
        Gizmos.DrawLine(q2.ToVector(), l2.point.ToVector());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
