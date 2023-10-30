using TrueSync;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace Game
{
    public class LogicPlayer
    {
        public int index;
        public int guid;
        public string name;

        public TSVector pos;
        public TSQuaternion rot;

        public FP speed = 100f;
        public void UpdateInput(InputData inputData)
        {
            //移动
            if (inputData.inputMoveAngle >= 0)
            {
                var moveDir = TSQuaternion.Euler(0, inputData.inputMoveAngle, 0) * TSVector.forward;

                moveDir *= speed * FrameManager.instance.frameTime;
                pos += moveDir;
            }
        }
    }
}