using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using C2SProtoInterface;
using CenterBase;
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
        public FP frameTime = 0.02f;
        public GameType gameType = GameType.Play;

        private originalData lorData = new originalData() { orPos = TSVector.forward, orRotation = default };
        private originalData rOrData = new originalData() { orPos = TSVector.back, orRotation = default };
        public Dictionary<int, S2CFrameData> frameDataInputs = new Dictionary<int, S2CFrameData>();
        private float timeCount;
        public int curServerFrame = -1;
        public int curClientFrame = -1;


        private int tracingFrameIndex;
        

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

            frmUpdate.Angle = InputManager.instance.inputData.inputMoveAngle._serializedValue;
            InputManager.instance.inputData.Clear();
            Debug.LogError($"发送帧数据 {frmUpdate}");
            ClientManager.instance.UDPSend(frmUpdate);
        }
        
        public void OnPrintFrames()
        {
            //lock (frameDataInputs)
            {
                
                var path = Directory.GetCurrentDirectory()+"\\FramesPrint.txt";
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                using (StreamWriter c = new StreamWriter(path, true))
                {

                    for (int i = 0; i < frameDataInputs.Count; i++)
                    {
                        string inputs = "";

                        var frame = frameDataInputs[i];

                        for (int j = 0; j < frame.Gids.Count; j++)
                        {
                            inputs += $" id:{frame.Gids[j]} yaw{frame.InputAngles[j]} ";
                        }
                        c.WriteLine($"[{frame.FrameIndex}] {inputs}");
                    }
                }
            }
        }
    }
}