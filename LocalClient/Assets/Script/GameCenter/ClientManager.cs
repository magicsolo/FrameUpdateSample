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
using System.Threading;
using C2SProtoInterface;
using TrueSync;
using UnityEngine;
using Google.Protobuf;
using UnityEngine.Assertions.Must;

public enum EInputEnum
{
    none,
    leftDir,
    rightDir,
    upDir,
    downDir,
}

public struct TCPInfo
{
    private Byte[] _datas;
    public EMessage msgType { get; private set; }
    int _msgOffset;
    int _msgLength;
    public unsafe void Init(Byte[] datas)
    {
        _datas = datas;
        _msgOffset = 0;
        fixed (byte* p = datas)
        {
            msgType = *(EMessage*)p;
            _msgOffset += sizeof(EMessage);
            _msgLength = *(int*)(p + _msgOffset);
            _msgOffset += sizeof(int);
        }
    }

    public T ParseMsgData<T>(MessageParser<T> parser)where T:IMessage<T>
    {
        return parser.ParseFrom(_datas, _msgOffset, _msgLength);
    }
}

[Serializable]
public struct FrameInputData
{
    public int index;
    public EInputEnum input;
}


public class ClientManager : BasicMonoSingle<ClientManager>
{
    private Dictionary<EMessage, Action<TCPInfo>> _callBacks = new Dictionary<EMessage, Action<TCPInfo>>();
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

    private Socket udpSocket = new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);
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
        IPAddress ipAddress = IPAddress.Parse(ip);
        ipPoint = new IPEndPoint(ipAddress, pot);
        udpSocket.Connect(ipPoint);
        tcpClient = new TcpClient(ip,8080);
        stream = tcpClient.GetStream();

        Init();
        //StartCoroutine("TCPUpdate");
        Thread TCP = new Thread(TCPUpdate);
        TCP.Start();
    }

    private TcpClient tcpClient;
    void TCPUpdate()
    {

        var readBytes = new byte[1024];
        while (true)
        {
            var readLength = stream.Read(readBytes, 0, readBytes.Length);
            if (readLength <=0)
                continue;
            RecieveTCPInfo(readBytes);
        }
    }

    void RecieveTCPInfo(byte[] data)
    {
        var tcpData = new TCPInfo();
        tcpData.Init(data);

        if (_callBacks.TryGetValue(tcpData.msgType,out var func))
            func(tcpData);
    }

    void OnLogin(Byte[] data)
    {
        var logInfo = S2CLogin.Parser.ParseFrom(data,sizeof(EMessage),data.Length - sizeof(EMessage));
        
    }
    public unsafe void SendTCPInfo(EMessage mesgType,IMessage data = null,Action<TCPInfo> callBack = null)
    {
        if (callBack != null)
        {
            _callBacks[mesgType] = callBack;
        }
        var bytesType = new Byte[sizeof(EMessage) + (data == null ? 0 : data.CalculateSize())];
        fixed (void* pbytes = bytesType)
            *(EMessage*)pbytes = mesgType;
        stream.Write(bytesType,0,bytesType.Length);
    }
    
    private void Init()
    {
        timeCount = 0;
        inputs.Clear();
    }
    private void Update()
    {
        // if (!isPlaying)
        //     return;
        // string message = "updating";
        // byte[] data = Encoding.UTF8.GetBytes(message);
        // stream.Read(data,0,data.Length);
        // timeCount += Time.deltaTime;
        // int curIndex = (int)Mathf.Floor(timeCount/(float)frameTime);
        // while (inputs.Count < (curIndex + 1))
        // {
        //     if ( FrameManager.instance.gameType != GameType.Pause && curClientFrame>=0)
        //         FrameManager.instance.UpdateInputData( inputs[curClientFrame] );
        //     inputs.Add( new FrameInputData(){ index = (curServerFrame + 1), input = EInputEnum.none } );
        // }
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
    private void OnDestroy()
    {
        if (tcpClient!=null&&tcpClient.Connected)
        {
            tcpClient.Close();
        }
    }
}
