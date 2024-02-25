using System;
using System.IO;
using System.Threading;
using C2SProtoInterface;
using Google.Protobuf;

namespace Game
{
    public class MatchDrive:LogicDrive
    {
        private Thread ud;
        private StreamWriter logWriter;
        private FileStream videoWriter;

        public override void Start(S2CStartGame servDt)
        {
            base.Start(servDt);
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
        public override void Stop()
        {
            base.Stop();
            EventManager.instance.UnRegEvent(EventKeys.LogicMatchUpdate,SaveFrameUpdate);
            CloseThread();
        }

        void ThreadUpdate()
        {
            ClientManager.instance.UDPConnect(this.servDt.Pot);
            while (true)
            {
                FrameManager.instance.RequireFrameDatas();
                match.Update();
                FrameManager.instance.SendFrameData();
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
    }
}