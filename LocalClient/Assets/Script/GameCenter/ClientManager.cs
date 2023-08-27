using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using Proto;
using TrueSync;
using UnityEngine;
using Debug = UnityEngine.Debug;

public enum EInputEnum
{
    none,
    leftDir,
    rightDir,
    upDir,
    downDir,
}

[Serializable]
public struct FrameInputData
{
    public int index;
    public EInputEnum input;
}


public class ClientManager : BasicMonoSingle<ClientManager>
{
    private FP frameTime => FrameManager.instance.frameTime;
    private float timeCount = 0;
    public List<FrameInputData> inputs = new List<FrameInputData>();

    public static int curServerFrame => instance.inputs.Count - 1;
    public static int curClientFrame => instance.inputs.Count - 2;
    
    private string ip = "192.168.50.22";
    private int pot = 8080;
    IPEndPoint ipPoint;
    private NetworkStream stream;
    private BinaryFormatter _serializer = new BinaryFormatter();
    private PlayerLogInfo _logInfo;
    bool isPlaying
    {
        get
        {
            switch (FrameManager.instance.gameType)
            {
                case GameType.Play:
                case GameType.Pause:
                case GameType.TraceFrame:
                    return true;
                    break;
                default:
                    return false;
                    break;
            }
        }
    }
    
    private void Start()
    {
        Init();
        // IPAddress ipAdress = IPAddress.Parse(ip);
        // ipPoint = new IPEndPoint(ipAdress, pot);
        var tcpClient = new TcpClient(ip,8080);

        stream = tcpClient.GetStream();
        string message = "Hello";
        byte[] data = Encoding.UTF8.GetBytes(message);
        stream.Write(data,0,data.Length);

        stream.Read(data, 0, data.Length);
        var dataStr = Encoding.UTF8.GetString(data);
        PlayerLogInfo deserializedWeatherForecast = JsonSerializer.Deserialize<PlayerLogInfo>(dataStr);
        using (MemoryStream ms = new MemoryStream(data))
        {
            IFormatter formatter = new BinaryFormatter();
            var obj = formatter.Deserialize(ms);
        }
        
        //int dataLength = stream.Read(data, 0, data.Length);
        object loginData = _serializer.Deserialize(stream);
        _logInfo = (PlayerLogInfo)loginData;

        //stream.Close();
    }
    
    
    private void Init()
    {
        timeCount = 0;
        inputs.Clear();
    }
    private void Update()
    {
        if (!isPlaying)
            return;
        string message = "updating";
        byte[] data = Encoding.UTF8.GetBytes(message);
        stream.Read(data,0,data.Length);
        timeCount += Time.deltaTime;
        int curIndex = (int)Mathf.Floor(timeCount/(float)frameTime);
        while (inputs.Count < (curIndex + 1))
        {
            if ( FrameManager.instance.gameType != GameType.Pause && curClientFrame>=0)
                FrameManager.instance.UpdateInputData( inputs[curClientFrame] );
            inputs.Add( new FrameInputData(){ index = (curServerFrame + 1), input = EInputEnum.none } );
        }
    }

    public void Play()
    {
        inputs.Clear();
        timeCount = 0;
    }
    
    public void Send(EInputEnum input)
    {
        if (!isPlaying)
            return;
        
        var inputs = instance.inputs;

        if (inputs.Count>0)
        {
            var inputData = inputs[inputs.Count - 1];
            inputData.input = input;
            inputs[inputs.Count - 1] = inputData;
        }
        
    }
    
    public void RequireFrames(int start,int end)
    {
        end = Math.Min(end, inputs.Count - 1);
        var reQuireFrames = inputs.GetRange(start, end - start + 1);
        FrameManager.instance.InsertFrames(reQuireFrames.ToArray());
    }

    // private int clientFrame = 0;
    //
    // public static void Recieve()
    // {
    //     if (instance.clientFrame < curFrame)
    //     {
    //         
    //     }
    // }
}
