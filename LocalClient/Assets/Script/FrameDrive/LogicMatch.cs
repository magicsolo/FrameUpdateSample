using System.Collections.Generic;
using CenterBase;

namespace FrameDrive
{
    public class LogicMatch:Single<LogicMatch>
    {
        private LogicPlayer[] _allPlayers;
        public ViewPlayerInfo[] viewPlayerInfo;
        public LogicPlayer[] allPlayers => _allPlayers;
        
        private Dictionary<int, LogicPlayer> dicPlayers = new Dictionary<int, LogicPlayer>();
        public int playerCount => allPlayers.Length;
        
        public void Init(PlayerFiled[] playerInfos)
        {
            ResetCharacters(playerInfos);   
        }

        public void Unit()
        {
            _allPlayers = null;
            viewPlayerInfo = null;
            dicPlayers.Clear();
        }

        void ResetCharacters(PlayerFiled[] playerInfos)
        {
            _allPlayers = new LogicPlayer[playerInfos.Length];
            viewPlayerInfo = new ViewPlayerInfo[playerInfos.Length];
            dicPlayers.Clear();
            for (int slot = 0; slot < _allPlayers.Length; slot++)
            {
                var plFiled = playerInfos[slot];
                var lgPl = new LogicPlayer(slot,this,plFiled);
                _allPlayers[slot] = lgPl;
                dicPlayers[lgPl.filed.info.guid] = lgPl;
            }
        }
        public void Update(FrameData frameData)
        {

            for (int i = 0; i < frameData.InputData.Length; i++)
            {
                var ipt = frameData.InputData[i];
                
                var player = _allPlayers[ipt.slot];
                var inputData = new FrameInputData() { input = (EInputEnum)ipt.input};
                inputData.inputMoveAngle = ipt.inputMoveAngle;
                player.UpdateInput(inputData);
            }

            lock (viewPlayerInfo)
            {
                for (int i = 0; i < _allPlayers.Length; i++)
                {
                    var pl = _allPlayers[i];
                    viewPlayerInfo[i] = new ViewPlayerInfo(pl.filed.info,pl.filed.data);
                }
            }
        }
        
        public LogicPlayer GetPlayer(int guid)
        {
            dicPlayers.TryGetValue(guid, out var findPlayer);
            return findPlayer;
        }
    }
}