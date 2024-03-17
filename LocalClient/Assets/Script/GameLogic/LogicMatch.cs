using System.Collections.Generic;
using C2SProtoInterface;
using CenterBase;
using TrueSync;
using UnityEngine;

namespace Game
{
    public class LogicMatch:Single<LogicMatch>
    {
        private Dictionary<int, LogicPlayer> _dicAllPlayers = new Dictionary<int, LogicPlayer>();
        private LogicPlayer[] _allPlayers;
        public LogicPlayer[] allPlayers => _allPlayers;
        
        public PlayerInfo[] viewPlayerInfo;
        private Dictionary<int, LogicPlayer> dicPlayers = new Dictionary<int, LogicPlayer>();
        
        public void Init(S2CStartGame servDt)
        {
            ResetCharacters(servDt);   
            FrameManager.instance.Init(servDt);
        }

        void ResetCharacters(S2CStartGame startData)
        {
            _allPlayers = new LogicPlayer[startData.Players.Count];
            viewPlayerInfo = new PlayerInfo[startData.Players.Count];
            _dicAllPlayers.Clear();
            dicPlayers.Clear();
            for (int slot = 0; slot < _allPlayers.Length; slot++)
            {
                var lgPl = new LogicPlayer(startData.Players[slot],slot);
                _allPlayers[slot] = lgPl;
                lgPl.playerData.pos = TSVector.zero;
                dicPlayers[lgPl.playerData.guid] = lgPl;
                viewPlayerInfo[slot] = lgPl.playerData;
                _dicAllPlayers[lgPl.playerData.guid] = lgPl;
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
                EventManager.instance.DispatchEvent(EventKeys.LogicMatchUpdate,curFrmData);
                //FrameManager.instance.SaveFrameUpdate(curFrmData);
                ++clientFrame;
                for (int i = 0; i < curFrmData.Gids.Count; i++)
                {
                    var gid = curFrmData.Gids[i];
                    var input = curFrmData.Inputs[i];
                    var angle = curFrmData.InputAngles[i];
            
                    var player = dicPlayers[gid];
                    var inputData = new InputData() { input = (EInputEnum)input};
                    inputData.inputMoveAngle._serializedValue = angle;
                    player.UpdateInput(inputData);
                }
            }

            FrameManager.instance.curClientFrame = clientFrame;

            lock (viewPlayerInfo)
            {
                for (int slot = 0; slot < _allPlayers.Length; slot++)
                {
                    var pl = _allPlayers[slot];
                    viewPlayerInfo[slot] = pl.playerData;
                }
            }
        }
        
        public LogicPlayer GetPlayer(int guid)
        {
            _dicAllPlayers.TryGetValue(guid, out var findPlayer);
            return findPlayer;
        }
    }
}