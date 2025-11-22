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
        public void StartDrive(MatchInfo matchInfo)
        {
            controler = new MatchLogicControler();
            FrameManager.instance.Init(matchInfo,controler);
            var savePath = Directory.GetCurrentDirectory() + "\\SaveData";
            Directory.CreateDirectory(savePath);
            var logPath = savePath + $"\\log_{matchInfo.guid}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}.txt";
            File.Delete(logPath);
            File.Create(logPath).Dispose();
            ClearStreamWriter(logPath,logWriter);
            logWriter = new StreamWriter(logPath);

            var videoPath = savePath + $"\\video_{matchInfo.guid}.bytes";
            if (videoWriter!=null)
                videoWriter.Close();
            File.Delete(videoPath);
            File.Create(videoPath).Dispose();
            videoWriter = new FileStream(videoPath,FileMode.Create,FileAccess.Write);
            eventRegister.AddRegister(EventKeys.LogicMatchUpdate,SaveFrameUpdate);
            
            eventRegister.RegistAll();
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
            FrameData frmUpdate = (FrameData)obj;
            //var lengthBytes = //BitConverter.GetBytes(length);
            //videoWriter.Write(lengthBytes,0,lengthBytes.Length);
            //videoWriter.Write(sendBytes,0,sendBytes.Length);
            //videoWriter.Flush();

            logWriter.WriteLine($"frm:{frmUpdate.frameIndex}");
            for (int i = 0; i < frmUpdate.InputData.Length; i++)
            {
                var data = frmUpdate.InputData[i];
                var input = data.input;
                var angle = data.inputMoveAngle;
                logWriter.WriteLine($"{i}:: input:{input} angle:{angle}");
            }
            logWriter.Flush();
        }
    }
}