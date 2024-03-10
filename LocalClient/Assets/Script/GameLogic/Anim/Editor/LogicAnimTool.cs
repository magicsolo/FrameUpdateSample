using System;
using System.Collections.Generic;
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
        private string savePath = "Assets/Art/AnimData/AnimatorAnimInfos.asset";

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
                    newInfo.fireFrame =  (int)(Mathf.Ceil(clip.events[0].time * 1000));
                var bindings = AnimationUtility.GetCurveBindings(clip);
                var settings = AnimationUtility.GetAnimationClipSettings(clip);
                Vector4 skillInfo = Vector4.zero;
                foreach (var binding in bindings)
                {
                    int a = 0;
                    switch (binding.propertyName)
                    {
                        case "skillInfo.x":
                            skillInfo.x = GetCurveFrameFloatValue(clip, binding);
                            break;
                        case "skillInfo.y":
                            skillInfo.y = GetCurveFrameFloatValue(clip, binding);
                            break;
                        case "skillInfo.z":
                            skillInfo.z = GetCurveFrameFloatValue(clip, binding);
                            break;
                        case "skillInfo.w":
                            skillInfo.w = GetCurveFrameFloatValue(clip, binding);
                            break;
                    }
                }

                newInfo.skillCheckArea.Set(skillInfo.x,skillInfo.y,skillInfo.z,skillInfo.w);
                newInfo.controllerName = controller.name;
                newInfo.stateName = state.state.name;
                newInfo.key = $"{newInfo.controllerName}_{newInfo.stateName}";
                newInfo.length = (int)(Mathf.Ceil(clip.length * 1000));
                assets.Add(newInfo);
            }
        }

        float GetCurveFrameFloatValue(AnimationClip clip,EditorCurveBinding binding)
        {
            float value = default;
            
            var curve = AnimationUtility.GetEditorCurve(clip, binding);
            var keys = curve.keys;
            if (keys.Length>=0)
                value = keys[0].value;
            
            return value;
        }
    }
}