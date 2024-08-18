using System;
using System.Collections.Generic;
using FrameDrive;
using Game;
using TrueSync;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace EditorGame
{
    public class LogicAnimTool : EditorWindow
    {
        private string animatorPath = "Assets/Art/Animators";
        public static string savePath = "Assets/Art/AnimData/AnimatorAnimInfos.asset";

        private List<LogicAnimInfo> assets = new List<LogicAnimInfo>();

        [MenuItem("GameTool/基于Animator的动画分析")]
        public static void ShowWindow()
        {
            var window = GetWindow<LogicAnimTool>();
            window.titleContent = new GUIContent("基于Animator的动画分析");
            window.Show();
        }

        private void OnGUI()
        {
            if (GUILayout.Button("生成Animator的动画文件"))
            {
                AnalyseAnimatorsAnim();
            }
        }

        void AnalyseAnimatorsAnim()
        {
            string[] paths = { animatorPath };

            assets.Clear();
            var assetsGId = AssetDatabase.FindAssets("t:AnimatorController", paths);
            foreach (var gId in assetsGId)
            {
                var animatorPath = AssetDatabase.GUIDToAssetPath(gId);
                var ctrler = AssetDatabase.LoadAssetAtPath<AnimatorController>(animatorPath);
                SaveAnimatorInfo(ctrler);
            }

            AssetLogicAnimAinfos info = new AssetLogicAnimAinfos();
            info.allInfos = assets.ToArray();
            AssetDatabase.CreateAsset(info, savePath);
        }

        void SaveAnimatorInfo(AnimatorController controller)
        {
           
            Dictionary<string, AnimationClip> dic = new Dictionary<string, AnimationClip>();
            foreach (var clip in controller.animationClips)
            {
                dic[clip.name] = clip;
            }
            foreach (var state in controller.layers[0].stateMachine.states)
            {
                LogicAnimInfo newInfo = new LogicAnimInfo();
                var animName = state.state.motion.name;
                var clip = dic[animName];
                if (clip.events.Length>0)
                    newInfo.fireFrame =  (clip.events[0].time / FrameManager.frameTime).AsInt();
                var bindings = AnimationUtility.GetCurveBindings(clip);
                Vector3 area = Vector3.zero;
                Vector2 pos = Vector2.zero;
                foreach (var binding in bindings)
                {
                    AnimationUtility.GetEditorCurve(clip, binding);
                    switch (binding.propertyName)
                    {
                        case "area.x":
                            newInfo.skillAreaX = GetCurveFrameFloatValue(clip, binding);
                            break;
                        case "area.y":
                            newInfo.skillAreaY = GetCurveFrameFloatValue(clip, binding);
                            break;
                        case "area.z":
                            newInfo.skillAreaZ = GetCurveFrameFloatValue(clip, binding);
                            break;
                        case "pos.x":
                            newInfo.skillPosX = GetCurveFrameFloatValue(clip, binding);
                            break;
                        case "pos.y":
                            newInfo.skillPosY = GetCurveFrameFloatValue(clip, binding);
                            break;
                        case "pos.z":
                            newInfo.skillPosZ = GetCurveFrameFloatValue(clip, binding);
                            break;
                        case "m_LocalPosition.x":
                            newInfo.posX = GetCurveFrameFloatValue(clip, binding);
                            break;
                        case "m_LocalPosition.y":
                            newInfo.posY = GetCurveFrameFloatValue(clip, binding);
                            break;
                        case "m_LocalPosition.Z":
                            newInfo.posZ = GetCurveFrameFloatValue(clip, binding);
                            break;
                    }
                }

                newInfo.controllerName = controller.name;
                newInfo.stateName = state.state.name;
                newInfo.key = $"{newInfo.controllerName}_{newInfo.stateName}";
                newInfo.length = (clip.length / FrameManager.frameTime).AsInt();
                assets.Add(newInfo);
            }
        }

        public static LogicFrameCurveInfo[] GetCurveFrameFloatValue(AnimationClip clip,EditorCurveBinding binding)
        {
            //float value = default;
            List<LogicFrameCurveInfo> lst = new List<LogicFrameCurveInfo>();
            var curve = AnimationUtility.GetEditorCurve(clip, binding);
            var keys = curve.keys;

            foreach (var key in keys)
            {
                var newFrame = new LogicFrameCurveInfo();
                newFrame.frame = (key.time / FrameManager.frameTime).AsInt();
                newFrame.Value = key.value;
                newFrame.inTan = key.inTangent;
                newFrame.outTan = key.outTangent;
                lst.Add(newFrame);
            }
            
            return lst.ToArray();
        }
    }
}