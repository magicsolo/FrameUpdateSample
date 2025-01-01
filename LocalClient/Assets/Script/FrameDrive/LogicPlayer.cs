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
        }
    }
    public struct PlayerInfo
    {
        public int guid;
        public string name;
        public int slot;

        public PlayerInfo(int gid, string name, int slot)
        {
            guid = gid;
            this.name = name;
            this.slot = slot;
        }

        public void Init(S2CPlayerData plData, int slot)
        {
            guid = plData.Guid;
            name = plData.Name;
            this.slot = slot;
        }
    }

    public class PlayerLogicData
    {
        public TSVector pos;
        public TSQuaternion rot;
        public FrameInputData inputData;
        public PlayAnimInfo aniInfo;
        public bool faceRight;

        public void DeepCoppy(PlayerLogicData targetLogicData)
        {
            targetLogicData.pos = pos;
            targetLogicData.rot = rot;
            targetLogicData.inputData = inputData;
            targetLogicData.aniInfo = aniInfo;
            targetLogicData.faceRight = faceRight;
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