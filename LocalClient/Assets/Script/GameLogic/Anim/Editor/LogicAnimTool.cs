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

                newInfo.controllerName = controller.name;
                newInfo.stateName = state.state.name;
                newInfo.key = $"{newInfo.controllerName}_{newInfo.stateName}";
                newInfo.length = (int)(Mathf.Ceil(clip.length * 1000));
                assets.Add(newInfo);
            }
        }
    }
}