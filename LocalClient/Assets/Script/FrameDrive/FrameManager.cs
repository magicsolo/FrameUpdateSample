using System;
using System.Collections.Generic;
using System.IO;
using C2SProtoInterface;
using CenterBase;
using Game;
using TrueSync;
using UnityEngine;


namespace FrameDrive
{
    public enum EInputEnum
    {
        none,
        fire,
    }

    public struct MatchInfo
    {
        public int guid;
        public PlayerInfo[] players;
        public int randomSeed;

        public MatchInfo(S2CMatchInfo matchInfo)
        {
            players = new PlayerInfo [matchInfo.Players.Count];
            for (int i = 0; i < matchInfo.Players.Count; i++)
            {
                var plInfo = matchInfo.Players[i];
                players[i] = new PlayerInfo(plInfo,i);
            }
            randomSeed = matchInfo.RandomSeed;
            guid = matchInfo.MatchGuid;
        }

        public MatchInfo(PlayerInfo[] players, int randomSeed = 0,int matchId = -1)
        {
            this.players = players;
            this.randomSeed = randomSeed;
            this.guid = matchId;
        }
    }

    public struct originalData
    {
        public TSVector orPos;
        public TSQuaternion orRotation;
    }

    public struct FrameInputData
    {
        public FP inputMoveAngle;
        public EInputEnum input;

        public void Clear()
        {
            input = EInputEnum.none;
            inputMoveAngle = -1;
        }
    }

    [SerializeField]
    public class FrameData
    {
        public int frameIndex;
        public FrameInputData[] InputData;

        public FrameData(int frameIndex,LogicPlayer[] allplayers)
        {
            this.frameIndex = frameIndex;
            InputData = new FrameInputData[allplayers.Length];

            for (int i = 0; i < InputData.Length; i++)
            {
                InputData[i].Clear();
            }
        }
    }

    public class FrameManager : BasicMonoSingle<FrameManager>
    {
        //1帧33毫秒
        public static readonly FP frameTime = (FP.One) * 33 / 1000;

        public Dictionary<int, FrameData> frameDataInputs = new Dictionary<int, FrameData>();
        public int curServerFrame { get; set; }
        public int clientRuningFrame { get; private set; }
        public FP curTime => GetTimeByFrame(clientRuningFrame); 

        private int tracingFrameIndex;
        public int playerCount => match.playerCount;
        public LogicMatch match => LogicMatch.instance;

        public static FP GetTimeByFrame(int frameIndex)
        {
            return Math.Max(frameIndex,0)*frameTime ; 
        }

        public void Init(MatchInfo matchInfo,MatchLogicControler controler = null)
        {
            clientRuningFrame = -1;
            curServerFrame = -1;
            frameDataInputs.Clear();
            match.Init(matchInfo,controler);
        }

        public void Unit()
        {
            frameDataInputs.Clear();
            match.Unit();
        }

        public FrameData AddFrameData(int frameIndex)
        {
            if (frameDataInputs.ContainsKey(frameIndex))
            {
                Debug.LogWarning($"FrameDrive -> TryAddContainedFrame {frameIndex}");
                return default;
            }

            var nFrm = new FrameData(frameIndex,match.allPlayers);
            EventManager.instance.DispatchEvent(EventKeys.LogicMatchUpdate,nFrm);
            frameDataInputs[frameIndex] = nFrm;

            return nFrm;
        }
        
        public void UpdateFrameData()
        {
            while (frameDataInputs.ContainsKey(clientRuningFrame+1))
            {
                ++clientRuningFrame;
                var frmDat = frameDataInputs[clientRuningFrame];
                match.Update(frmDat);
            }
        }

        // public (S2CStartGame, List<S2CFrameData>) LoadVideo()
        // {
        //     var path = Directory.GetCurrentDirectory() + "\\video.bytes";
        //     List<S2CFrameData> frames = new List<S2CFrameData>();
        //     S2CStartGame startInfo;
        //     
        //     var strm = new FileStream(path, FileMode.Open, FileAccess.Read);
        //     byte[] rdBytes = new byte[1024 * 1024];
        //     strm.Read(rdBytes, 0, sizeof(Int32));
        //     int dataLength = BitConverter.ToInt32(rdBytes, 0);
        //     strm.Read(rdBytes, 0, dataLength);
        //     startInfo = S2CStartGame.Parser.ParseFrom(rdBytes,0,dataLength);
        //     
        //
        //     while (strm.Read(rdBytes, 0, sizeof(Int32))>0)
        //     {
        //         dataLength = BitConverter.ToInt32(rdBytes, 0);
        //         S2CFrameData frmDt;
        //         if (dataLength > 0)
        //         {
        //             strm.Read(rdBytes, 0, dataLength);
        //             try
        //             {
        //                 frmDt = S2CFrameData.Parser.ParseFrom(rdBytes, 0, dataLength);
        //             }
        //             catch
        //             {
        //                 break;
        //             }
        //         }
        //         else
        //         {
        //             frmDt = new S2CFrameData();
        //         }
        //
        //         frames.Add(frmDt);
        //     }
        //     return (startInfo, frames);
        // }

        public void PlayVideoFrame(List<S2CFrameData> frames,int serverFrame)
        {
            // curServerFrame = serverFrame;
            // foreach (var frm in frames)
            // {
            //     frameDataInputs[frm.FrameIndex] = frm;
            // }
        }

        public static bool isInFrame(FP passedTime,FP checkFrameTime)
        {
            return passedTime >= checkFrameTime && passedTime < (checkFrameTime + frameTime);
        }
        
    }
}