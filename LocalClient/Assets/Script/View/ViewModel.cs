using System;
using System.Collections.Generic;
using System.Linq;
using CenterBase;
using Game;
using UnityEngine;
namespace Script
{
    public class ViewModel:BasicMonoSingle<ViewModel>
    {
        
        [SerializeField]
        private ViewPlayer sampPlayeer;
        private List<ViewPlayer> players = new List<ViewPlayer>();

        public Transform playersNode;

        private void Start()
        {
            transform.position = Vector3.zero;
            transform.rotation = default;
            
            GameObject plsNode = new GameObject("PlayersNode");
            playersNode = plsNode.transform;
            plsNode.transform.SetParent(transform,true);
            
            players.Clear();
            ViewPlayer[] pls = GetComponentsInChildren<ViewPlayer>(true);
            players.AddRange(pls);
            ResetPlayers();
        }

        public void ResetPlayers(LogicPlayer[] logicPlayers = null)
        {
            int idx = 0;
            if (logicPlayers!=null)
            {
                for (; idx < logicPlayers.Length; idx++)
                {
                    var lgPl = logicPlayers[idx];
                    ViewPlayer vpl;
                    if (players.Count<idx)
                    {
                        vpl = players[idx];
                    }
                    else
                    {
                        vpl = GameObject.Instantiate(sampPlayeer);
                        vpl.transform.SetParent(playersNode);
                    }

                    vpl.lgPlayer = lgPl;
                    vpl.gameObject.SetActive(true);
                    ++idx;
                }
            }
            

            for (; idx < players.Count; idx++)
            {
                players[idx].gameObject.SetActive(false);
            }
        }
    }
}