using C2SProtoInterface;
using TrueSync;
using UnityEngine;

namespace Game
{
    public struct PlayerInfo
    {
        public int index;
        public int guid;
        public string name;
        public int slot;

        public TSVector pos;
        public TSQuaternion rot;
        public InputData InputData;

        public PlayAnimInfo aniInfo;

        public bool faceRight;
    }

    public struct PlayAnimInfo
    {
        public string stateName;
        public int totalTime;
        public int curTime;
        public int startTime;
    }
    
    public class LogicPlayer
    {
        public PlayerInfo playerData = new PlayerInfo();
        private PlayerFSM fsm;
        public EPlayerState curStateType => (EPlayerState)fsm.curState.stateType;
        public FP speed = 10f;

        public LogicPlayer(S2CPlayerData sPlData,int slot)
        {
            playerData.guid = sPlData.Guid;
            playerData.name = sPlData.Name;
            playerData.slot = slot;
            fsm = new PlayerFSM(this);
        }
        public void UpdateInput(InputData inputData)
        {
            playerData.InputData = inputData;
            fsm.curState.Update();
            
        }
    }
}