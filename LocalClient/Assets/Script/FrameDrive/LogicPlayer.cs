using C2SProtoInterface;
using Game;
using TrueSync;

namespace FrameDrive
{
    public class PlayerFiled
    {
        public PlayerInfo info;
        public PlayerLogicInfo data = new PlayerLogicInfo();

        public PlayerFiled(PlayerInfo plInfo)
        {
            info = plInfo;
            data.life = new Fraction<int>(3, 3);
        }
    }
    public struct PlayerInfo
    {
        public int guid;
        public string playerName;
        public int slot;

        public PlayerInfo(int gid, string playerName, int slot)
        {
            guid = gid;
            this.playerName = playerName;
            this.slot = slot;
        }

        public PlayerInfo(S2CPlayerData plData, int slot)
        {
            guid = plData.Guid;
            playerName = plData.Name;
            this.slot = slot;
        }
    }

    public class PlayerLogicInfo
    {
        public TSVector pos;
        public TSQuaternion rot;
        public FrameInputData inputData;
        public PlayAnimInfo aniInfo;
        public Fraction<int> life;
    }

    public struct PlayAnimInfo
    {
        public string stateName;
        public int totalFrame;
        public int passedFrame;
        public int startFrame;
    }
    
    public class LogicPlayer
    {
        public int slot;
        public PlayerFSM fsm;
        public EPlayerState curStateType => (EPlayerState)fsm.curState.stateType;
        public FP speed = 10f;

        public LogicMatch match;
        public PlayerFiled filed;

        public LogicPlayer(int slot,LogicMatch match,PlayerInfo playerFiled)
        {
            this.slot = slot;
            this.match = match;
            filed = new PlayerFiled(playerFiled);
            fsm = new PlayerFSM(this);
        }
        public void UpdateInput(FrameInputData frameInputData)
        {
            filed.data.inputData = frameInputData;
            fsm.Update();
        }
    }
}