using System;
using System.Collections.Generic;
using System.IO;
using C2SProtoInterface;
using CenterBase;
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
        public int slot;
        public FP inputMoveAngle;
        public EInputEnum input;

        public void Clear()
        {
            inputMoveAngle = -1;
            input = EInputEnum.none;
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
        }
    }

    public static class ExProto
    {
        public static FrameInputData GetInputData(this S2CFrameData frmInputData, int idx)
        {
            return new FrameInputData { input = (EInputEnum)frmInputData.Inputs[idx], inputMoveAngle = FP.FromRaw(frmInputData.InputAngles[idx]) };
        }
    }

    public class FrameManager : BasicMonoSingle<FrameManager>
    {
        //1帧33毫秒
        public static readonly FP frameTime = (FP.One) * 33 / 1000;

        public Dictionary<int, FrameData> frameDataInputs = new Dictionary<int, FrameData>();
        public int curServerFrame { get; private set; }
        public int curClientFrame { get; set; }
        public FP curTime => Math.Max(curClientFrame,0)*frameTime ; 

        private int tracingFrameIndex;
        public int playerCount => match.playerCount;
        public LogicMatch match = new LogicMatch();
        

        public void Init(PlayerFiled[] playerFileds )
        {
            curClientFrame = -1;
            frameDataInputs.Clear();
            match.Init(playerFileds);
        }

        public void Unit()
        {
            curClientFrame = -1;
            frameDataInputs.Clear();
            match.Unit();
        }

        public FrameData AddFrameData(int frameIndex)
        {
            if (frameDataInputs.ContainsKey(frameIndex))
            {
                Debug.LogError($"FrameDrive -> TryAddContainedFrame {frameIndex}");
                return default;
            }

            var nFrm = new FrameData(frameIndex,match.allPlayers);
            frameDataInputs[frameIndex] = nFrm;

            return nFrm;
        }
        
        public void UpdateFrameData()
        {
            while (frameDataInputs.ContainsKey(curClientFrame+1))
            {
                ++curClientFrame;
                var frmDat = frameDataInputs[curClientFrame];
                match.Update(frmDat);
            }
        }

        public (S2CStartGame, List<S2CFrameData>) LoadVideo()
        {
            var path = Directory.GetCurrentDirectory() + "\\video.bytes";
            List<S2CFrameData> frames = new List<S2CFrameData>();
            S2CStartGame startInfo;
            
            var strm = new FileStream(path, FileMode.Open, FileAccess.Read);
            byte[] rdBytes = new byte[1024 * 1024];
            strm.Read(rdBytes, 0, sizeof(Int32));
            int dataLength = BitConverter.ToInt32(rdBytes, 0);
            strm.Read(rdBytes, 0, dataLength);
            startInfo = S2CStartGame.Parser.ParseFrom(rdBytes,0,dataLength);
            

            while (strm.Read(rdBytes, 0, sizeof(Int32))>0)
            {
                dataLength = BitConverter.ToInt32(rdBytes, 0);
                S2CFrameData frmDt;
                if (dataLength > 0)
                {
                    strm.Read(rdBytes, 0, dataLength);
                    try
                    {
                        frmDt = S2CFrameData.Parser.ParseFrom(rdBytes, 0, dataLength);
                    }
                    catch
                    {
                        break;
                    }
                }
                else
                {
                    frmDt = new S2CFrameData();
                }

                frames.Add(frmDt);
            }
            return (startInfo, frames);
        }

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