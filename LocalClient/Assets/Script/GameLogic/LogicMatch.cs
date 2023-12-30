using System.Collections.Generic;
using C2SProtoInterface;
using TrueSync;
using UnityEngine;

namespace Game
{
    public class LogicMatch
    {
        private Dictionary<int, LogicPlayer> dicAllPlayers = new Dictionary<int, LogicPlayer>();
        private LogicPlayer[] allPlayers;
        public PlayerInfo[] viewPlayerInfo;
        private Dictionary<int, LogicPlayer> dicPlayers = new Dictionary<int, LogicPlayer>();

        
        public void Init(S2CStartGame servDt)
        {
            ResetCharacters(servDt);   
        }

        void ResetCharacters(S2CStartGame startData)
        {
            allPlayers = new LogicPlayer[startData.Players.Count];
            viewPlayerInfo = new PlayerInfo[startData.Players.Count];
            dicAllPlayers.Clear();
            dicPlayers.Clear();
            for (int slot = 0; slot < allPlayers.Length; slot++)
            {
                var lgPl = new LogicPlayer(startData.Players[slot],slot);
                allPlayers[slot] = lgPl;
                lgPl.playerData.pos = TSVector.zero;
                dicPlayers[lgPl.playerData.guid] = lgPl;
                viewPlayerInfo[slot] = lgPl.playerData;
                dicAllPlayers[lgPl.playerData.guid] = lgPl;
            }
        }
        public void Update()
        {
            var clientFrame = FrameManager.instance.curClientFrame;
            var servFrame = FrameManager.instance.curServerFrame;
            while (clientFrame < servFrame && FrameManager.instance.frameDataInputs.TryGetValue(clientFrame + 1, out var curFrmData))
            {
                if (clientFrame +1 != curFrmData.FrameIndex)
                {
                    Debug.LogError($"帧数据Idx与实际数据Idx不一致 第【{clientFrame + 1}】 数据上帧标记【{curFrmData.FrameIndex}】");
                    break;
                }
                ++clientFrame;
                for (int i = 0; i < curFrmData.Gids.Count; i++)
                {
                    var gid = curFrmData.Gids[i];
                    var input = curFrmData.Inputs[i];
                    var angle = curFrmData.InputAngles[i];
            
                    var player = dicPlayers[gid];
                    var inputData = new InputData() { input = (EInputEnum)input };
                    inputData.inputMoveAngle = angle;
                    player.UpdateInput(inputData);
                }
            }

            FrameManager.instance.curClientFrame = clientFrame;

            // lock (viewPlayerInfo)
            // {
            //     for (int slot = 0; slot < allPlayers.Length; slot++)
            //     {
            //         var pl = allPlayers[slot];
            //         viewPlayerInfo[slot] = pl.playerData;
            //     }
            // }
        }
    }
}