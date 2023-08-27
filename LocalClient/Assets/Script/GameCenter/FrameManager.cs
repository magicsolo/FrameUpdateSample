using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using TrueSync;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public enum GameType
{
    Play,//正常游戏
    PlayBack,//重播
    TraceFrame,//追帧
    Pause,//掉线
}

public struct originalData
{
    public TSVector orPos;
    public TSQuaternion orRotation;
}

public class FrameManager : BasicMonoSingle<FrameManager>
{
    public FP frameTime = 0.02f;
    public CharacterModel character;
    public GameType gameType = GameType.Play;

    private originalData orData = default;
    private List<FrameInputData> frameDataInputs = new List<FrameInputData>();
    private float timeCount;
    public int curClientFrame = -1;

    public override void OnAwake()
    {
        base.OnAwake();
        

    }


    private void Start()
    {
        orData = new originalData() { orPos = character.tsTrans.position, orRotation = character.tsTrans.rotation };
    }

    public void UpdateInputData(FrameInputData proto)
    {
        switch (gameType)
        {
            case GameType.Play:
                if (proto.index != curClientFrame + 1)
                    TraceFrame(proto);
                else
                {
                    character.UpdateInput(proto.input);
                    curClientFrame = proto.index;
                }
                break;
            case GameType.TraceFrame:
                frameDataInputs.Insert(frameDataInputs.Count, proto);
                break;
            case GameType.PlayBack:
                character.UpdateInput(proto.input);
                curClientFrame = proto.index;
                break;
        }
    }

    private void Update()
    {
        switch (gameType)
        {
            case GameType.PlayBack:
                UpdatePlayBack();
                break;
        }
    }
    

    private void UpdatePlayBack()
    {
        timeCount += Time.deltaTime;
        int curIndex = (int)Mathf.Floor(timeCount / (float)frameTime);
        while (curIndex < frameDataInputs.Count && curClientFrame < curIndex)
        {
            curClientFrame = curIndex;
            if (curClientFrame >= 0)
                UpdateInputData( frameDataInputs[curIndex] );
        }
    }

    private void OnGUI()
    {
        GUIStyle btnStyle;

        //TODO 
        btnStyle = GUI.skin.button;
        btnStyle.fontSize = 120;
        switch (gameType)
        {
            case GameType.Play:
                if (GUILayout.Button("重播",btnStyle ))
                {
                    frameDataInputs.Clear();
                    PlayBack();
                }

                if (GUILayout.Button("暂停",btnStyle))
                {
                    Pause();
                }
                break;
            case GameType.PlayBack:
                if (GUILayout.Button("开始",btnStyle))
                {
                    Play();
                }
                break;
            case GameType.Pause:
                if (GUILayout.Button("继续",btnStyle))
                {
                    Continue();
                }
                break;
        }
        
        
    }

    private void PlayBack()
    {
        frameDataInputs.Clear();
        frameDataInputs.AddRange(ClientManager.instance.inputs.ToArray());
        gameType = GameType.PlayBack;
        character.tsTrans.position = orData.orPos;
        character.tsTrans.rotation = orData.orRotation;
        timeCount = 0;
        curClientFrame = 0;
    }

    public void Pause()
    {
        gameType = GameType.Pause;
    }
    
    private void Play()
    {
        gameType = GameType.Play;
        curClientFrame = -1;
        character.tsTrans.position = orData.orPos;
        character.tsTrans.rotation = orData.orRotation;
        ClientManager.instance.Play();
    }

    
    private int tracingFrameIndex;

    private void Continue()
    {
        gameType = GameType.Play;
    }
    
    private void TraceFrame(FrameInputData proto)
    {
        gameType = GameType.TraceFrame;
        tracingFrameIndex = 0;
        frameDataInputs.Clear();
        frameDataInputs.Add(proto);
        StartCoroutine("UpdateTraceFrame");
    }

    private IEnumerator UpdateTraceFrame()
    {
        //一帧追完
        while (tracingFrameIndex < frameDataInputs.Count)
        {
            //找到要追的那一帧
            var frame = frameDataInputs[tracingFrameIndex];
            
            while (curClientFrame >= frame.index && tracingFrameIndex<frameDataInputs.Count)
            {
                frame = frameDataInputs[tracingFrameIndex];
                ++tracingFrameIndex;
            }

            //找了半天发下来的都是过时的数据，别追了
            if (tracingFrameIndex>=frameDataInputs.Count || curClientFrame >= frame.index)
            {
                break;
            }
            
            //补全从当前帧到实际帧的中间帧
            if (curClientFrame + 1 != frame.index)
            {
                RequireFromeTo(curClientFrame + 1, frame.index - 1);
                yield return new WaitForSeconds(0.04f) ;
                continue;
            }
            character.UpdateInput(frame.input);
            curClientFrame = frame.index;
        }
        gameType = GameType.Play;
    }

    private void RequireFromeTo(int startFrame, int endFrame)
    {
        ClientManager.instance.RequireFrames(startFrame,endFrame);
    }

    public void InsertFrames(FrameInputData[] datas)
    {
        if (datas == null || datas.Length <1)
            return;
        int insertIdx = 0;
        for (int inputIdx = 0; inputIdx < datas.Length; ++inputIdx)
        {
            var insertData = datas[inputIdx];
            if (insertData.index <= curClientFrame)
                continue;

            for (; insertIdx < frameDataInputs.Count; ++insertIdx)
            {
                var enumInputData = frameDataInputs[insertIdx];

                if(insertData.index == enumInputData.index)
                    break;
                else if (insertData.index < enumInputData.index)
                {
                    frameDataInputs.Insert(insertIdx, datas[inputIdx]);
                    break;
                }
            }
            //从尾部插入
            if (insertIdx == frameDataInputs.Count )
            {
                frameDataInputs.Insert(insertIdx, datas[inputIdx]);
                break;
            }
        }
        
    }
        
}
