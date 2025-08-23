using System.Collections.Generic;
using CenterBase;
using Game;

namespace FrameDrive
{
    public class LogicMatch:Single<LogicMatch>
    {
        private LogicPlayer[] _allPlayers;
        public FramePlayerInfo[] framePlayerInfos;
        public LogicPlayer[] allPlayers => _allPlayers;
        
        private Dictionary<int, LogicPlayer> dicPlayers = new Dictionary<int, LogicPlayer>();
        public int playerCount => allPlayers.Length;
        MatchLogicControler _controler;
        public bool isInput = true;
        
        public void Init(PlayerFiled[] playerInfos,MatchLogicControler controler = null)
        {
            ResetCharacters(playerInfos);
            _controler = controler;
            isInput = true;
        }

        public void Unit()
        {
            _allPlayers = null;
            framePlayerInfos = null;
            dicPlayers.Clear();
        }

        void ResetCharacters(PlayerFiled[] playerInfos)
        {
            _allPlayers = new LogicPlayer[playerInfos.Length];
            framePlayerInfos = new FramePlayerInfo[playerInfos.Length];
            dicPlayers.Clear();
            for (int slot = 0; slot < _allPlayers.Length; slot++)
            {
                var plFiled = playerInfos[slot];
                var lgPl = new LogicPlayer(slot,this,plFiled);
                _allPlayers[slot] = lgPl;
                dicPlayers[lgPl.filed.info.guid] = lgPl;
            }
            
            RefreshViewInfo();
        }
        public void Update(FrameData frameData)
        {

            if (_controler != null)
            {
                _controler.Update();
            }
            for (int i = 0; i < frameData.InputData.Length; i++)
            {
                var ipt = frameData.InputData[i];
                
                var player = _allPlayers[i];
                var inputData = new FrameInputData();
                inputData.Clear();
                if (isInput)
                {
                    inputData.input = (EInputEnum)ipt.input;
                    inputData.inputMoveAngle = ipt.inputMoveAngle;    
                }
                player.UpdateInput(inputData);
            }

            lock (framePlayerInfos)
            {
                RefreshViewInfo();
            }
        }

        void RefreshViewInfo()
        {
            for (int i = 0; i < _allPlayers.Length; i++)
            {
                var pl = _allPlayers[i];
                framePlayerInfos[i] = new FramePlayerInfo(pl);
            }
        }
        
        public LogicPlayer GetPlayer(int guid)
        {
            dicPlayers.TryGetValue(guid, out var findPlayer);
            return findPlayer;
        }
    }
}