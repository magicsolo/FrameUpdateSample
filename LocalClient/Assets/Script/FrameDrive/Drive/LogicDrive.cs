using C2SProtoInterface;
using FrameDrive;

namespace Game
{
    public abstract class LogicDrive
    {
        protected virtual int spaceTime => 1;
        public int curClientFrame => FrameManager.instance.curClientFrame;
        public int curServerFrame => FrameManager.instance.curServerFrame;
    }
}