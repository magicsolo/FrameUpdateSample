using System;
using System.IO;
using System.Threading;
using C2SProtoInterface;
using FrameDrive;
using Google.Protobuf;

namespace Game
{
    public class ServerMatchDrive:LogicDrive
    {
        private Thread ud;
        private StreamWriter logWriter;
        private FileStream videoWriter;
        private S2CStartGame servDt;

        public void Start(S2CStartGame servDt)
        {
            this.servDt = servDt;
            CloseThread();

            ud = new Thread(ThreadUpdate);
            ud.Start();
            
            var logPath = Directory.GetCurrentDirectory() + "\\log.txt";
            ClearStreamWriter(logPath,logWriter);
            logWriter = new StreamWriter(logPath);

            var videoPath = Directory.GetCurrentDirectory() + "\\video.bytes";
            if (videoWriter!=null)
                videoWriter.Close();
            videoWriter = new FileStream(videoPath,FileMode.Create,FileAccess.Write);
            var startBytes = servDt.ToByteArray();
            Int32 length = startBytes.Length;
            var lengthBytes = BitConverter.GetBytes(length);
            videoWriter.Write(lengthBytes,0,lengthBytes.Length);
            videoWriter.Write(startBytes,0,startBytes.Length);
            
            EventManager.instance.RegEvent(EventKeys.LogicMatchUpdate,SaveFrameUpdate);
        }

        public void SaveFrameUpdate(object obj)
        {
            S2CFrameData frmUpdate = (S2CFrameData)obj;
            var sendBytes = frmUpdate.ToByteArray();
            Int32 length = sendBytes.Length;
            var lengthBytes = BitConverter.GetBytes(length);
            videoWriter.Write(lengthBytes,0,lengthBytes.Length);
            videoWriter.Write(sendBytes,0,sendBytes.Length);
            videoWriter.Flush();

            logWriter.WriteLine($"frm:{frmUpdate.FrameIndex}");
            for (int i = 0; i < frmUpdate.Gids.Count; i++)
            {
                var gid = frmUpdate.Gids[i];
                var input = frmUpdate.Inputs[i];
                var angle = frmUpdate.InputAngles[i];
                logWriter.WriteLine($"{gid}:: input:{input} angle:{angle}");
            }
            logWriter.Flush();
        }
        
        void ClearStreamWriter(string path,StreamWriter writer)
        {
            if (writer!=null)
                writer.Close();
            File.Delete(path);
            File.Create(path).Dispose();
        }
        public void Stop()
        {
            EventManager.instance.UnRegEvent(EventKeys.LogicMatchUpdate,SaveFrameUpdate);
            CloseThread();
        }

        void ThreadUpdate()
        {
            ClientManager.instance.UDPConnect(this.servDt.Pot);
            while (true)
            {
                S2CFrameUpdate frmServerData = ClientManager.instance.UDPReceive();
                foreach (var sFrmData in frmServerData.FrameDatas)
                {
                    FrameData dt = FrameManager.instance.AddFrameData(sFrmData.FrameIndex);
                    for (int i = 0; i < FrameManager.instance.playerCount; i++)
                    {
                        var inputData = dt.InputData[i];
                        inputData.input = (EInputEnum)sFrmData.Inputs[i];
                        inputData.inputMoveAngle._serializedValue = sFrmData.InputAngles[i];
                    }
                }
                FrameManager.instance.UpdateFrameData();
                SendFrameData();
                Thread.Sleep(spaceTime);
            }
        }
        
        void CloseThread()
        {
            if (ud!=null && ud.IsAlive)
            {
                ud.Abort();
                ud = null;
            }
        }
        
        public void SendFrameData()
        {
            C2SFrameUpdate frmUpdate = new C2SFrameUpdate();

            var requireStart = Math.Min(curClientFrame + 1, curServerFrame);
            var requireEnd = curServerFrame;
            if (!FrameManager.instance.frameDataInputs.ContainsKey(requireStart))
            {
                for (; requireEnd <= curServerFrame; requireEnd++)
                {
                    if (FrameManager.instance.frameDataInputs.ContainsKey(requireEnd))
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
    }
}