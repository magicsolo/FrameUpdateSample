using System;
using System.Collections.Generic;
using System.Linq;
using C2SProtoInterface;
using CenterBase;
using FrameDrive;
using Game;
using Google.Protobuf.Reflection;
using UnityEngine;
namespace Script
{
    public class ViewModel:BasicMonoSingle<ViewModel>
    {
        
        [SerializeField]
        private ViewPlayer sampPlayeer;

        private LogicMatch match;
        private List<ViewPlayer> players = new List<ViewPlayer>();
        public ViewPlayerInfo[] playerInfos;

        Transform _playersNode;

        private void Start()
        {
            transform.position = Vector3.zero;
            transform.rotation = default;
            
            GameObject plsNode = new GameObject("PlayersNode");
            _playersNode = plsNode.transform;
            plsNode.transform.SetParent(transform,true);
            
            players.Clear();
            ViewPlayer[] pls = GetComponentsInChildren<ViewPlayer>(true);

            foreach (var vp in pls)
            {
                vp.gameObject.SetActive(false);
            }
            players.AddRange(pls);
            gameObject.SetActive(false);
            //ResetPlayers();
        }

        public void Init()
        {
            this.match = FrameManager.instance.match;
            gameObject.SetActive(true);
            playerInfos = new ViewPlayerInfo[match.playerCount];
            foreach (var pl in players)
            {
                pl.gameObject.SetActive(false);
            }
            
            for (int slot = 0; slot < FrameManager.instance.playerCount; slot++)
            {
                ViewPlayer vpl;

                if (players.Count > slot)
                {
                    vpl = players[slot];
                }
                else
                {
                    vpl = GameObject.Instantiate(sampPlayeer);
                    vpl.transform.SetParent(_playersNode);
                    vpl.Init(slot);
                    players.Add(vpl);
                }
                vpl.gameObject.SetActive(true);
            }
        }

        public void Unit()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {
            lock (match.viewPlayerInfo)
            {
                for (int slot = 0; slot < match.viewPlayerInfo.Length; slot++)
                {
                    var viewInfo = match.viewPlayerInfo[slot];
                    playerInfos[slot] = viewInfo;
                }
            }
        }


        public ViewPlayerInfo GetPlayerInfo(int slot)
        {
            return (playerInfos != null && slot < playerInfos.Length) ? playerInfos[slot] : default;
        }
        // public void ResetPlayers(LogicPlayer[] logicPlayers = null)
        // {
        //     int idx = 0;
        //     if (logicPlayers != null)
        //     {
        //         for (; idx < logicPlayers.Length; idx++)
        //         {
        //             var lgPl = logicPlayers[idx];
        //             ViewPlayer vpl;
        //             if (players.Count>idx)
        //             {
        //                 vpl = players[idx];
        //             }
        //             else
        //             {
        //                 vpl = GameObject.Instantiate(sampPlayeer);
        //                 vpl.transform.SetParent(_playersNode);
        //                 players.Add(vpl);
        //             }
        //
        //             vpl.lgPlayer = lgPl;
        //             vpl.gameObject.SetActive(true);
        //         }
        //     }
        //     
        //
        //     for (; idx < players.Count; idx++)
        //     {
        //         players[idx].gameObject.SetActive(false);
        //     }
        // }
    }
}