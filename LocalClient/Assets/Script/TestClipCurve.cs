using System;
using UnityEditor;
using UnityEngine;

public class TestClipCurve:MonoBehaviour
{
    [SerializeField]
    private AnimationCurve curve;
    [SerializeField]
    private AnimationCurve curve_Ex;
    [SerializeField]
    private AnimationCurve TestCurve;
    [SerializeField]
    private Keyframe[] frames;

    [SerializeField]
    private AnimationClip clip;
    
    private void Update()
    {
        var bindings = AnimationUtility.GetCurveBindings(clip);

        foreach (var bd in bindings)
        {
            if (bd.propertyName == "m_LocalPosition.x")
            {
                var animCurve = AnimationUtility.GetEditorCurve(clip, bd);
                curve = new AnimationCurve();
                for (int i = 0; i < animCurve.keys.Length; i++)
                {
                    var keyFrame = animCurve.keys[i];
                    curve.AddKey(keyFrame);
                    curve_Ex.AddKey(new Keyframe(keyFrame.time, keyFrame.value, keyFrame.inTangent,keyFrame.outTangent));
                }
            }
        }

        frames = TestCurve.keys;
    }
}