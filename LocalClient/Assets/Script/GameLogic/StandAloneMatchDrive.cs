using System;
using System.IO;
using System.Threading;
using C2SProtoInterface;
using FrameDrive;
using Google.Protobuf;
using TrueSync;

namespace Game
{
    public class StandAloneMatchDrive:LogicDrive
    {
        private Thread ud;
        private StreamWriter logWriter;
        private FileStream videoWriter;
        private int _spaceTime;
        protected override int spaceTime => _spaceTime;
        
        public void Start(PlayerFiled[] playerFileds)
        {
            FrameManager.instance.Init(playerFileds);
            _spaceTime = (FrameManager.frameTime*1000).AsInt();
            CloseThread();

            ud = new Thread(Update);
            ud.Start();
            
            var logPath = Directory.GetCurrentDirectory() + "\\log.txt";
            ClearStreamWriter(logPath,logWriter);
            logWriter = new StreamWriter(logPath);

            var videoPath = Directory.GetCurrentDirectory() + "\\video.bytes";
            if (videoWriter!=null)
                videoWriter.Close();
            videoWriter = new FileStream(videoPath,FileMode.Create,FileAccess.Write);
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

        public void Update()
        {
            var nxtFrame = FrameManager.instance.AddFrameData(FrameManager.instance.curClientFrame + 1);
            nxtFrame.InputData[0].input = InputManager.instance.inputData.input;
            nxtFrame.InputData[0].inputMoveAngle = InputManager.instance.inputData.inputMoveAngle;
            FrameManager.instance.UpdateFrameData();
        }
        
        void CloseThread()
        {
            if (ud!=null && ud.IsAlive)
            {
                ud.Abort();
                ud = null;
            }
        }
    }
}