using C2SProtoInterface;
using FrameDrive;

namespace Game
{
    public abstract class LogicDrive
    {
        protected virtual int spaceTime => 11;
        public int curClientFrame => FrameManager.instance.clientRuningFrame;
        public int curServerFrame => FrameManager.instance.curServerFrame;
    }
}