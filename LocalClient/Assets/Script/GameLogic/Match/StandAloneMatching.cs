using C2SProtoInterface;
using CenterBase;
using FrameDrive;
using Script;
using UnityEngine;

namespace Game
{
    public class StandAloneMatching:LogicState
    {
        private StandAloneMatchDrive driver = new StandAloneMatchDrive();
        public StandAloneMatching(LogicFSM fsm) : base(ELogicType.StandAloneMatching, fsm)
        {
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            driver.Start((MatchInfo) param);
            ViewModel.instance.Init();
        }

        public override void OnGUIUpdate()
        {
            base.OnGUIUpdate();
            if (GUILayout.Button("Leave"))
            {
                logicFsm.ChangeState(ELogicType.StandAloneRoom);
            }
            
            FramePlayerInfo playerWiner = default;
            FramePlayerInfo playerLoseer = default;
            bool gameEnd = false;
            foreach (var vPlayer in ViewModel.instance.playerInfos)
            {
                if (vPlayer.state == EPlayerState.Death && vPlayer.aniInfo.passedFrame > vPlayer.aniInfo.totalFrame)
                {
                    gameEnd = true;
                    playerLoseer = vPlayer;

                }
                else
                {
                    playerWiner = vPlayer;
                }
            }

            if (gameEnd)
            {
                GUILayout.Label($"比赛结束 胜者:{playerWiner.info.playerName} 败者:{playerLoseer.info.playerName}", txtStyle);
            }
        }

        public override void Exit()
        {
            base.Exit();
            driver.StopDrive();
            ViewModel.instance.Unit();
        }
    }
}