using Game;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameLogic))]
public class GameLogicEditor: Editor
{
    public override void OnInspectorGUI()
    {
        var comp = (GameLogic)target;
        var curStateType = comp.fsm?.curState?.stateType ?? 0;
        GUILayout.Label($"当前状态 :{(ELogicType)curStateType}");
        DrawDefaultInspector();
    }
}