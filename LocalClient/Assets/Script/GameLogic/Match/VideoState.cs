using System.Collections.Generic;
using System.IO;
using C2SProtoInterface;
using CenterBase;
using Script;
using UnityEngine;

namespace Game
{
    public class VideoState:LogicState
    {
        private string fileName;
        public VideoState( LogicFSM fsm) : base(ELogicType.Video, fsm){}
        private VideoDrive driver = new VideoDrive();

        protected override void BeforeEnter()
        {
            base.BeforeEnter();
            fileName = param as string;
        }

        public override void Exit()
        {
            base.Exit();
            //driver.Stop();
            ViewModel.instance.Unit();
        }

        public override void Update()
        {
            base.Update();
            driver.Update();
        }

        public override void OnGUIUpdate()
        {
            base.OnGUIUpdate();
            if (GUILayout.Button("停止"))
            {
                logicFsm.ChangeState(ELogicType.Disconnection);
            }
        }
        
        
    }
}