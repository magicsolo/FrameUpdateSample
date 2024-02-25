using System;
using System.IO;
using System.Threading;
using C2SProtoInterface;
using Game;
using Google.Protobuf;
using UnityEditor.PackageManager;
using UnityEngine.UIElements;

namespace Game
{
    public abstract class LogicDrive
    {
        protected int spaceTime = 1;
        protected S2CStartGame servDt;
        public LogicMatch match => LogicMatch.instance;
        
        public virtual void Start(S2CStartGame servDt)
        {
            this.servDt = servDt;
            match.Init(this.servDt);
        }
        public virtual void Stop()
        {
        }
    }
}