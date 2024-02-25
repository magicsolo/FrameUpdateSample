using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using C2SProtoInterface;
using UnityEngine.PlayerLoop;

namespace Game
{
    public class VideoDrive:LogicDrive
    {
        private S2CStartGame startInfo;
        private List<S2CFrameData> allFrames;
        private List<S2CFrameData> curFrameFrames = new List<S2CFrameData>();
        private int curServerFrameIndex;
        private int updateFrame = 1;

        public void Start(S2CStartGame startInfo, List<S2CFrameData> frames)
        {
            base.Start(startInfo);
            this.startInfo = startInfo;
            allFrames = frames;
            curServerFrameIndex = -1;
        }
        
        public void Update()
        {
            if (curServerFrameIndex >= allFrames.Count -1)
                return;
            
            curFrameFrames.Clear();
            int udFrames = Math.Min(allFrames.Count - curServerFrameIndex, updateFrame);
            for (int i = 1; i <= udFrames; i++)
                curFrameFrames.Add(allFrames[curServerFrameIndex + i]);
            curServerFrameIndex += curFrameFrames.Count;

            FrameManager.instance.PlayVideoFrame(allFrames,curServerFrameIndex);
            match.Update();
        }
    }
}