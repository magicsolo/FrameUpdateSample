using System.Collections;
using System.Collections.Generic;
using FrameDrive;
using Game;
using Script;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ViewPlayer))]
public class ViewPlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var vPlayer = (ViewPlayer)target;
        if (ViewModel.instance == null || LogicMatch.instance==null)
        {
            EditorMode(vPlayer);
        }
        else
        {
            Runinning(vPlayer);
        }
    }

    void EditorMode(ViewPlayer vPlayer)
    {
        if (GUILayout.Button("绑定"))
        {
            vPlayer.BindObj();
        }
            
    }

    void Runinning(ViewPlayer vPlayer)
    {
        var playerInfo = ViewModel.instance.GetPlayerInfo(vPlayer.slot);
        var guid = playerInfo.info.guid;
        var logicPlayer = LogicMatch.instance.GetPlayer(guid);
        if (logicPlayer!=null)
        {
            GUILayout.Label($"State:{logicPlayer.curStateType} angle:{playerInfo.FrameInputData.inputMoveAngle.AsFloat()} btn:{playerInfo.FrameInputData.input}");
        }
    }
}
