using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using Sirenix.Utilities;
using TrueSync;
using UnityEditor;
using UnityEngine;

namespace EditorGame
{
    public class TestAnimCurve : EditorWindow
    {
        private AssetLogicAnimAinfos asset;
        private AnimationClip viewClipObj;
        private SerializedObject viewClip;
        public DrawAssetInfo assetInfoDrawer;
        
        [MenuItem("GameTool/动画测试面板/曲线值验证")]
        public static void ShowWindow()
        {
            var window = GetWindow<TestAnimCurve>();
            window.titleContent = new GUIContent("基于Animator的动画分析");
            window.Show();
        }

        void LoadAsset()
        {
            asset = AssetDatabase.LoadAssetAtPath<AssetLogicAnimAinfos>(LogicAnimTool.savePath);
        }

        private void OnGUI()
        {
            if (asset == null)
            {
                LoadAsset();
            }

            if (assetInfoDrawer.SafeIsUnityNull())
            {
                assetInfoDrawer = null;
                var gameObj = GameObject.Find("DrawAssetInfoObject");
                if (gameObj != null)
                {
                    assetInfoDrawer = gameObj.GetComponent<DrawAssetInfo>();
                    if (assetInfoDrawer == null)
                    {
                        assetInfoDrawer = gameObj.AddComponent<DrawAssetInfo>();
                    }
                }
            }
            
            if (asset == null)
            {
                EditorGUILayout.LabelField("NoAsset");
                return;
            }
            AnimationCurve animDataCur = new AnimationCurve();

            foreach (var info in asset.allInfos)
            {
                // foreach (var frame in info.posX)
                // {
                //     animDataCur.AddKey(new Keyframe(frame.frame * FrameManager.frameTime.AsFloat(), frame.Value.AsFloat(), frame.inTan.AsFloat(), frame.outTan.AsFloat()));
                // }
                //
                if (info.stateName.Contains("hurt"))
                {
                    assetInfoDrawer.SetDrawInfo(info);
                    // EditorGUILayout.LabelField(info.stateName);
                    // EditorGUILayout.CurveField(animDataCur);
                    break;
                }
            }

            EditorGUI.BeginChangeCheck();
            viewClipObj = EditorGUILayout.ObjectField("clip",viewClipObj, typeof(AnimationClip),true) as AnimationClip;
            if (EditorGUI.EndChangeCheck())
            {
                viewClip = new SerializedObject(viewClipObj);
            }

            LogicFrameCurveInfo[] curveInfos = null;
            if (viewClipObj!=null)
            {
                var bindings = AnimationUtility.GetCurveBindings(viewClipObj);
                foreach (var binding in bindings)
                {
                    if (binding.propertyName == "m_LocalPosition.x")
                    {

                        var curve = AnimationUtility.GetEditorCurve(viewClipObj, binding);
                        EditorGUILayout.CurveField(curve);
                        
                        AnimationCurve copyCurve = new AnimationCurve();

                        foreach (var key in curve.keys)
                        {
                            FP time = key.time;
                            FP v = key.value;
                            FP inTan = key.inTangent;
                            FP oTan = key.outTangent;
                            copyCurve.AddKey(new Keyframe(time.AsFloat(),v.AsFloat(),inTan.AsFloat(),oTan.AsFloat()));
                        }
                        GUILayout.Label("CopyCurve:");
                        EditorGUILayout.CurveField(copyCurve);
                        
                        curveInfos = LogicAnimTool.GetCurveFrameFloatValue(viewClipObj, binding);
                        break;
                    }
                }
            }
        }

        Vector2 CalculateBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector2 p = uuu * p0; // 第一项
            p += 3 * uu * t * p1; // 第二项
            p += 3 * u * tt * p2; // 第三项
            p += ttt * p3; // 第四项

            return p;
        }

    }

}
