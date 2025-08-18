using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using C2SProtoInterface;
using FrameDrive;
using Google.Protobuf;

namespace Game
{
    public abstract class BaseLogic
    {
        public virtual bool OnUpdate()
        {
            return true;
        }
    }

    public class RuningLogic : BaseLogic
    {
        public override bool OnUpdate()
        {
            foreach (var player in LogicMatch.instance.allPlayers)
            {
                if (player.curStateType == EPlayerState.Death)
                {
                    LogicMatch.instance.isInput = false;
                    return true;
                }
            }
            return false;
        }
    }

    public class DeathingLogic : BaseLogic
    {
    }
    public class MatchLogicControler
    {
        Queue<BaseLogic> queue = new Queue<BaseLogic>();

        public MatchLogicControler()
        {
            queue.Enqueue(new RuningLogic());
            queue.Enqueue(new DeathingLogic());
        }
        public void Update()
        {
            if (queue.Count > 0)
            {
                if (queue.Peek().OnUpdate())
                {
                    queue.Dequeue();
                }
            }
        }
    }
    public abstract class BaseMatchDrive : LogicDrive
    {
        private StreamWriter logWriter;
        private FileStream videoWriter;
        private Thread ud;
        private EventRegister eventRegister = new EventRegister();
        private bool _threadUpdate = false;

        public MatchLogicControler controler;
        public void StartDrive(PlayerFiled[] playerFileds)
        {
            controler = new MatchLogicControler();
            FrameManager.instance.Init(playerFileds,controler);
            var logPath = Directory.GetCurrentDirectory() + "\\log.txt";
            ClearStreamWriter(logPath,logWriter);
            logWriter = new StreamWriter(logPath);

            var videoPath = Directory.GetCurrentDirectory() + "\\video.bytes";
            if (videoWriter!=null)
                videoWriter.Close();
            videoWriter = new FileStream(videoPath,FileMode.Create,FileAccess.Write);
            eventRegister.AddRegister(EventKeys.LogicMatchUpdate,SaveFrameUpdate);
            CloseThread();
            _threadUpdate = true;
            ud = new Thread(Update);
            ud.Start();
        }

        ~BaseMatchDrive()
        {
            _threadUpdate = false;
        }

        public void StopDrive()
        {
            _threadUpdate = false;
            eventRegister.UnregistAll();
            CloseThread();
        }

        void Update()
        {
            while (_threadUpdate)
            {
                OnUpdate();
                Thread.Sleep(spaceTime);
            }
        }
        protected abstract void OnUpdate();
        
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