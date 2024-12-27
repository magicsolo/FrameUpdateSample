using System;
using System.IO;
using System.Threading;
using C2SProtoInterface;
using FrameDrive;
using Google.Protobuf;

namespace Game
{
    public abstract class BaseMatchDrive : LogicDrive
    {
        private StreamWriter logWriter;
        private FileStream videoWriter;
        private Thread ud;
        private EventRegister eventRegister = new EventRegister();
        public void StartDrive(PlayerFiled[] playerFileds)
        {
            FrameManager.instance.Init(playerFileds);
            var logPath = Directory.GetCurrentDirectory() + "\\log.txt";
            ClearStreamWriter(logPath,logWriter);
            logWriter = new StreamWriter(logPath);

            var videoPath = Directory.GetCurrentDirectory() + "\\video.bytes";
            if (videoWriter!=null)
                videoWriter.Close();
            videoWriter = new FileStream(videoPath,FileMode.Create,FileAccess.Write);
            eventRegister.AddRegister(EventKeys.LogicMatchUpdate,SaveFrameUpdate);
            CloseThread();
            ud = new Thread(Update);
            ud.Start();
        }

        public void StopDrive()
        {
            eventRegister.UnregistAll();
            CloseThread();
        }

        protected abstract void Update();
        
        void CloseThread()
        {
            if (ud!=null && ud.IsAlive)
            {
                ud.Abort();
                ud = null;
            }
        }
        
        void ClearStreamWriter(string path,StreamWriter writer)
        {
            if (writer!=null)
                writer.Close();
            File.Delete(path);
            File.Create(path).Dispose();
        }
        
        void SaveFrameUpdate(object obj)
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
    }
}