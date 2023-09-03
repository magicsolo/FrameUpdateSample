using System;
using System.Collections;
using System.Collections.Generic;
using C2SProtoInterface;
using UnityEngine;

public struct PlayerData
{
    //-1:未连接 0:连接中 大于0:已登录
    public int guid;
}

public class GameLogic : MonoBehaviour
{
    enum LogicState
    {
        Login,
        Gaming,
    }

    private LogicState state = LogicState.Login;
    public PlayerData playerData = new PlayerData() { guid = -1 };

    

    private void OnGUI()
    {
        switch (state)
        {
            case LogicState.Login:
                ShowLogin();
                break;
            case LogicState.Gaming:
                ShowGaming();
                break;
        }
        

    }

    private void ShowLogin()
    {
        if (playerData.guid < 0)
        {
            if (GUILayout.Button("连接"))
                Login();
        }
        else
        {
            GUILayout.Label("连接中");
        }
    }

    private void ShowGaming()
    {
        GUIStyle btnStyle;

        //TODO 
        btnStyle = GUI.skin.button;
        btnStyle.fontSize = 120;
        switch (FrameManager.instance.gameType)
        {
            case GameType.Play:
                if (GUILayout.Button("重播",btnStyle ))
                {
                    FrameManager.instance.PlayBack();
                }

                if (GUILayout.Button("暂停",btnStyle))
                {
                    FrameManager.instance.Pause();
                }
                break;
            case GameType.PlayBack:
                if (GUILayout.Button("开始",btnStyle))
                {
                    FrameManager.instance.Play();
                }
                break;
            case GameType.Pause:
                if (GUILayout.Button("继续",btnStyle))
                {
                    FrameManager.instance.Continue();
                }
                break;
        }
    }

    // Start is called before the first frame updat
    private void Login()
    {
        playerData.guid = 0;
        ClientManager.instance.SendTCPInfo(EMessage.Login,new C2SLogin(),OnLogin);
        FrameManager.instance.Play();
    }
    void OnLogin(TCPInfo data)
    {
        var logInfo = data.ParseMsgData(S2CLogin.Parser);
        playerData.guid = logInfo.Guid;
        state = LogicState.Gaming;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    
}
