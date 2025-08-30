using System;
using System.Collections.Generic;
using CenterBase;
using FPPhysic;
using TrueSync;
using UnityEditor;
using UnityEngine;

namespace Game
{
    public abstract class DrawData
    {
        public Material material;
        public Mesh mash;
        public Matrix4x4 matrix;

        public DrawData(Material mat)
        {
            mash = new Mesh();
            material = mat;
        }
        public abstract void RefreshData();

    }

    public class DrawPlane : DrawData
    {
        public FPPlane plane;

        public DrawPlane(Material mat) : base(mat)
        {
        }

        public void Init(FPPlane plane)
        {
            this.plane = plane;
        }

        public override void RefreshData()
        {
            
        }
    }
    public class FPDrawTool:BasicMonoSingle<FPDrawTool>
    {
        public Dictionary<string, TSVector> drawPoints = new Dictionary<string, TSVector>();
        private Transform pointTransParent;

        public override void OnAwake()
        {
            base.OnAwake();
            pointTransParent = new GameObject("PointTransParent").transform;
            pointTransParent.SetParent(transform);
        }

        // public void SetPoint(TSVector point, string name)
        // {
        //     var target = pointTransParent.Find(name);
        //     if (target==null)
        //     {
        //         target = new GameObject(name).transform;
        //         target.SetParent(pointTransParent,false);
        //         GUIContent iconContent = EditorGUIUtility.IconContent("sv_icon_dot1_pix16_gizmo");
        //         Texture2D defaultIcon = iconContent.image as Texture2D;
        //         EditorGUIUtility.SetIconForObject(target.gameObject,defaultIcon);
        //     }
        //
        //     target.position = point.ToVector();
        //     target.gameObject.name = name;
        //     
        // }

        public void RemovePoint(string name)
        {
            var target = pointTransParent.Find(name);
            if (target!=null)
            {
                Destroy(target.gameObject);
            }
        }

        private void OnDrawGizmos()
        {
            foreach (var VARIABLE in drawPoints)
            {
                
            }
        }
    }
}