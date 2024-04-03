using C2SProtoInterface;

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