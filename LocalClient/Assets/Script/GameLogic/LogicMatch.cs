using System.Collections.Generic;
using C2SProtoInterface;
using TrueSync;

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
                FrameManager.instance.curClientFrame = curFrmData.FrameIndex;
            }

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