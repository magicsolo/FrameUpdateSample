using System;
using System.Collections.Generic;
using CenterBase;
using UnityEngine;

namespace Game
{
    public class AssetLogicAnimAinfosManager: BasicMonoSingle<AssetLogicAnimAinfosManager>
    {
        public Dictionary<string, LogicAnimInfo> allAnimInfos;
        [SerializeField]
        private AssetLogicAnimAinfos asset;

        private void OnEnable()
        {
            allAnimInfos = new Dictionary<string, LogicAnimInfo>();
            foreach (var info in asset.allInfos)
                allAnimInfos.Add(info.key,info);
        }

        public LogicAnimInfo GetAnimInfo(string assetName)
        {
            if (allAnimInfos.TryGetValue(assetName,out var info))
            {
                return info;
            }

            return default;
        }
    }
}