using System.Collections;
using System.Collections.Generic;
using CenterBase;
using TrueSync;
using UnityEngine;

namespace Game
{
    public class InputManager : BasicMonoSingle<InputManager>
    {
        public InputData inputData = default;

        public override void OnAwake()
        {
            base.OnAwake();
            inputData.Clear();
        }

        // Update is called once per frame
        void Update()
        {
            Vector2 dir = Vector2.zero;
            if (Input.GetKey(KeyCode.A))
            {
                dir.x -= 0.5f;
            }else if (Input.GetKey(KeyCode.D))
            {
                dir.x += 0.5f;
            }
            if (Input.GetKey(KeyCode.W))
            {
                dir.y += 0.5f;
            }else if (Input.GetKey(KeyCode.S))
            {
                dir.y -= 0.5f;
            }

            //dir = Vector2.one;
            if (dir.sqrMagnitude > 0)
            {
                var vDir = Vector3.Cross(new Vector3(dir.x, 0, dir.y), Vector3.forward);
                var angle = Vector2.Angle(Vector2.up, dir)*Mathf.Sign( vDir.y*-1);
                if (angle<0)
                    angle += 360;
                inputData.inputMoveAngle = FP.FromFloat(angle);
            }
        }
    }
}

