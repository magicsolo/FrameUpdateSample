using System.Collections;
using System.Collections.Generic;
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
            return;
        var playerInfo = ViewModel.instance.GetPlayerInfo(vPlayer.slot);
        var guid = playerInfo.guid;
        var logicPlayer = LogicMatch.instance.GetPlayer(guid);
        GUILayout.Label($"State:{logicPlayer.curStateType}");
    }
}
