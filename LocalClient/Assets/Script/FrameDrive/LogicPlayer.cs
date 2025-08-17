using C2SProtoInterface;
using Game;
using TrueSync;

namespace FrameDrive
{
    public class PlayerFiled
    {
        public PlayerInfo info;
        public PlayerLogicData data = new PlayerLogicData();

        public PlayerFiled(PlayerInfo plInfo)
        {
            info = plInfo;
            data.life = new Fraction<int>(10, 10);
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

        public void Init(S2CPlayerData plData, int slot)
        {
            guid = plData.Guid;
            playerName = plData.Name;
            this.slot = slot;
        }
    }

    public class PlayerLogicData
    {
        public TSVector pos;
        public TSQuaternion rot;
        public FrameInputData inputData;
        public PlayAnimInfo aniInfo;
        public Fraction<int> life;

        public void DeepCoppy(PlayerLogicData targetLogicData)
        {
            targetLogicData.pos = pos;
            targetLogicData.rot = rot;
            targetLogicData.inputData = inputData;
            targetLogicData.aniInfo = aniInfo;
            life = targetLogicData.life;
        }
    }

    public struct PlayAnimInfo
    {
        public string stateName;
        public int totalFrame;
        public int curFrame;
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

        public LogicPlayer(int slot,LogicMatch match,PlayerFiled playerFiled)
        {
            this.slot = slot;
            this.match = match;
            filed = playerFiled;
            fsm = new PlayerFSM(this);
        }
        public void UpdateInput(FrameInputData frameInputData)
        {
            filed.data.inputData = frameInputData;
            fsm.Update();
        }
    }
}