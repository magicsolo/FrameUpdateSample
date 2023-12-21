using System.Threading;
using C2SProtoInterface;
using Game;
using UnityEditor.PackageManager;
using UnityEngine.UIElements;

namespace Game
{
    public class LogicDrive
    {
        private int spaceTime = 1;
        private Thread ud;
        private S2CStartGame servDt;
        public LogicMatch match = new LogicMatch();
        public void Start(S2CStartGame servDt)
        {
            this.servDt = servDt;
            CloseThread();

            match.Init(this.servDt);

            ud = new Thread(ThreadUpdate);
            ud.Start();
        }

        public void End()
        {
            CloseThread();
        }
        
        void CloseThread()
        {
            if (ud!=null && ud.IsAlive)
            {
                ud.Abort();
                ud = null;
            }
        }
        void ThreadUpdate()
        {
            ClientManager.instance.UDPConnect(this.servDt.Pot);
            while (true)
            {
                FrameManager.instance.RequireFrameDatas();
                match.Update();
                
                Thread.Sleep(spaceTime);
            }
        }
        
    }
}