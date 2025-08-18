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
        public FP curTime => Math.Max(clientRuningFrame,0)*frameTime ; 

        private int tracingFrameIndex;
        public int playerCount => match.playerCount;
        public LogicMatch match => LogicMatch.instance;
        
        

        public void Init(PlayerFiled[] playerFileds,MatchLogicControler controler = null )
        {
            clientRuningFrame = -1;
            curServerFrame = -1;
            frameDataInputs.Clear();
            match.Init(playerFileds,controler);
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