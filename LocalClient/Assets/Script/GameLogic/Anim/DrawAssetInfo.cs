using System;
using System.Collections;
using System.Collections.Generic;
using FrameDrive;
using Game;
using UnityEditor;
using UnityEngine;

public class DrawAssetInfo : MonoBehaviour
{
    [SerializeField]
    private LogicAnimInfo drawTarget;
    public void SetDrawInfo(LogicAnimInfo info)
    {
        drawTarget = info;
    }

    private void OnDrawGizmos()
    {
        var info = drawTarget;
        if (info.length<=1)
        {
            return;
        }
        Handles.color = Color.red;
        for (int i = 1; i < info.length; i++)
        {
            var lstX = info.GetCurveValue(info.posX,i - 1);
            Vector3 lstPos = new Vector3(((i - 1) * FrameManager.frameTime).AsFloat(),lstX.AsFloat(),0);
            var curX = info.GetCurveValue(info.posX,i);
            Vector3 curPos = new Vector3((i * FrameManager.frameTime).AsFloat(), curX.AsFloat(), 0);
            Handles.DrawLine(lstPos, curPos);
                
        }
    }
}
