using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using C2SProtoInterface;
using CenterBase;
using Google.Protobuf;
using TrueSync;
using UnityEngine;


namespace Game
{
    public struct originalData
    {
        public TSVector orPos;
        public TSQuaternion orRotation;
    }

    public struct InputData
    {
        public FP inputMoveAngle;
        public EInputEnum input;

        public void Clear()
        {
            inputMoveAngle = -1;
            input = EInputEnum.none;
        }

        public InputData(long iAngle, int iInput)
        {
            inputMoveAngle = FP.FromRaw(iAngle);
            input = (EInputEnum)iInput;
        }
    }

    public static class ExProto
    {
        public static InputData GetInputData(this S2CFrameData frmInputData, int idx)
        {
            return new InputData { input = (EInputEnum)frmInputData.Inputs[idx], inputMoveAngle = FP.FromRaw(frmInputData.InputAngles[idx]) };
        }
    }

    public class FrameManager : BasicMonoSingle<FrameManager>
    {
        //1帧33毫秒
        public static readonly FP frameTime = (FP.One) * 33 / 1000;
        public GameType gameType = GameType.Play;

        public Dictionary<int, S2CFrameData> frameDataInputs = new Dictionary<int, S2CFrameData>();
        public int curServerFrame { get; private set; }
        public int curClientFrame { get; set; }
        public FP curTime => Math.Max(curClientFrame,0)*frameTime ; 

        private int tracingFrameIndex;
        

        public void Init(S2CStartGame enterInfo)
        {
            curServerFrame = -1;
            curClientFrame = -1;
            
        }
        
        //TODO 晚点拆分挪到LogicMatch里
        public void RequireFrameDatas()
        {
            //lock (frameDataInputs)
            {
                S2CFrameUpdate frmServerData = ClientManager.instance.UDPReceive();
                if (frmServerData != null)
                {
                    curServerFrame = Math.Max(curServerFrame, frmServerData.CurServerFrame);
                    foreach (var frmDt in frmServerData.FrameDatas)
                    {
                        frameDataInputs[frmDt.FrameIndex] = frmDt;
                    }
                }
            }
        }

        public void SendFrameData()
        {
            C2SFrameUpdate frmUpdate = new C2SFrameUpdate();

            var requireStart = Math.Min(curClientFrame + 1, curServerFrame);
            var requireEnd = curServerFrame;
            if (!frameDataInputs.ContainsKey(requireStart))
            {
                for (; requireEnd <= curServerFrame; requireEnd++)
                {
                    if (frameDataInputs.ContainsKey(requireEnd))
                    {
                        requireEnd -= 1;
                        break;
                    }
                }
            }
            else
            {
                requireStart = -1;
                requireEnd = -1;
            }


            frmUpdate.Start = requireStart;
            frmUpdate.End = requireEnd;

            frmUpdate.Input = (int)InputManager.instance.inputData.input;
            frmUpdate.Angle = InputManager.instance.inputData.inputMoveAngle._serializedValue;
            ClientManager.instance.UDPSend(frmUpdate);
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
            curServerFrame = serverFrame;
            foreach (var frm in frames)
            {
                frameDataInputs[frm.FrameIndex] = frm;
            }
        }

        public static bool isInFrame(FP passedTime,FP checkFrameTime)
        {
            return passedTime >= checkFrameTime && passedTime < (checkFrameTime + frameTime);
        }
    }
}