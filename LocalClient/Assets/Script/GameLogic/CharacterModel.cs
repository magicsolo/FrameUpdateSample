using System;
using System.Collections;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;

namespace Game
{
    public class CharacterModel : MonoBehaviour
    {
        public FP speed = 1f;
        public TSTransform tsTrans;
        public bool isLeftTeam;

        private void Awake()
        {
            tsTrans = GetComponent<TSTransform>();
        }

        public void UpdateInput(InputData inputData)
        {
            if (tsTrans == null)
                return;
            //移动
            if (inputData.inputMoveAngle >= 0)
            {
                var moveDir = TSQuaternion.Euler(0, inputData.inputMoveAngle, 0) * TSVector.forward;

                moveDir *= speed * FrameManager.instance.frameTime;
                tsTrans.position += moveDir;
            }
        }
    }
}
