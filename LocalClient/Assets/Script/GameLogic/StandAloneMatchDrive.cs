using System;
using System.IO;
using System.Threading;
using C2SProtoInterface;
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
        
        public override void Start(S2CStartGame servDt)
        {
            base.Start(servDt);
            _spaceTime = (FrameManager.frameTime*1000).AsInt();
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
                S2CFrameUpdate frmServerData = new S2CFrameUpdate();//ClientManager.instance.UDPReceive();
                frmServerData.CurServerFrame = FrameManager.instance.curClientFrame + 2;
                var newFrame = new S2CFrameData();
                newFrame.FrameIndex = FrameManager.instance.curClientFrame + 1;
                for (int i = 0; i < match.allPlayers.Length; i++)
                {
                    var player = match.allPlayers[i];
                    newFrame.Gids.Add(player.playerData.guid);
                    if (i == 0)
                    {
                        newFrame.Inputs.Add((int)InputManager.instance.inputData.input);
                        newFrame.InputAngles.Add(InputManager.instance.inputData.inputMoveAngle._serializedValue);
                    }
                    newFrame.Inputs.Add(0);
                    newFrame.InputAngles.Add((new FP(-1))._serializedValue);
                }
                
                frmServerData.FrameDatas.Add(newFrame);
                FrameManager.instance.UpdateFrameDatas(frmServerData);
                match.Update();
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