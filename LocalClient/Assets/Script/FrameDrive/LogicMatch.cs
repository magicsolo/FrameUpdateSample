using System.Collections.Generic;
using CenterBase;
using Game;
using TrueSync;
using UnityEngine;

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
        
        public void Init(MatchInfo matchInfo,MatchLogicControler controler = null)
        {
            TSRandom.Init();
            TSRandom.instance.Initialize(matchInfo.randomSeed); 
            ResetCharacters(matchInfo.players);
            _controler = controler;
            isInput = true;
        }

        public void Unit()
        {
            _allPlayers = null;
            framePlayerInfos = null;
            dicPlayers.Clear();
        }

        void ResetCharacters(PlayerInfo[] playerInfos)
        {
            _allPlayers = new LogicPlayer[playerInfos.Length];
            framePlayerInfos = new FramePlayerInfo[playerInfos.Length];
            dicPlayers.Clear();
            for (int slot = 0; slot < _allPlayers.Length; slot++)
            {
                var plInfo = playerInfos[slot];
                var lgPl = new LogicPlayer(slot,this,plInfo);
                _allPlayers[slot] = lgPl;
                dicPlayers[lgPl.filed.info.guid] = lgPl;
                var randomX = TSRandom.instance.Next(0, GameManager.halfArea.x/4);
                var randomY = TSRandom.instance.Next(0, GameManager.halfArea.y/4);
                var offset = new TSVector2(randomX, randomY);
                TSVector2 pos = default;
                switch (slot % 4)
                {
                    case 0:
                        pos = GameManager.halfArea / 2;
                        break;
                    case 1:
                        pos = -GameManager.halfArea / 2;
                        offset *= -1;
                        break;
                    case 2:
                        pos = GameManager.halfArea / 2;
                        pos.x *= -1;
                        offset.x *= -1;
                        break;
                    case 3:
                    default:
                        pos = GameManager.halfArea / 2;
                        pos.y *= -1;
                        offset.y *= -1;
                        break;
                }

                pos += offset;
                lgPl.filed.data.pos = new TSVector(pos.x, 0, pos.y);
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