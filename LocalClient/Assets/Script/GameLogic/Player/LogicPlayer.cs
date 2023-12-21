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
    }
    public class LogicPlayer
    {
        public PlayerInfo playerData = new PlayerInfo();


        public FP speed = 10f;

        public LogicPlayer(S2CPlayerData sPlData,int slot)
        {
            playerData.guid = sPlData.Guid;
            playerData.name = sPlData.Name;
            playerData.slot = slot;
        }
        public void UpdateInput(InputData inputData)
        {
            //移动
            if (inputData.inputMoveAngle >= 0)
            {
                var moveDir = TSQuaternion.Euler(0, inputData.inputMoveAngle, 0) * TSVector.forward;

                moveDir *= speed * FrameManager.instance.frameTime;
                playerData.pos += moveDir;
            }
        }
    }
}