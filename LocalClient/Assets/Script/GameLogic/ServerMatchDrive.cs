using System;
using System.Threading;
using C2SProtoInterface;
using FrameDrive;
using UnityEngine;

namespace Game
{
    public class ServerMatchDrive : BaseMatchDrive
    {
        private MatchInfo matchInfo;
        static public int sendIndex = 0;
        public void Start(MatchInfo matchInfo)
        {
            sendIndex = 0;
            this.matchInfo = matchInfo;
            PlayerFiled[] playerFileds = new PlayerFiled[this.matchInfo.players.Count];

            for (int i = 0; i < playerFileds.Length; i++)
            {
                var plData = this.matchInfo.players[i];
                var plFiled = new PlayerFiled(new PlayerInfo(plData.guid, plData.name, i));
                plFiled.data = new PlayerLogicData();
                playerFileds[i] = plFiled;
            }

            StartDrive(playerFileds);
            ClientManager.instance.UDPConnect(8091);

        }

        protected override void OnUpdate()
        {
            S2CFrameUpdate frmServerData = ClientManager.instance.UDPReceive();

            if (frmServerData?.FrameDatas != null)
            {
                FrameManager.instance.curServerFrame = frmServerData.CurServerFrame;

                foreach (var sFrmData in frmServerData.FrameDatas)
                {
                    FrameData dt = FrameManager.instance.AddFrameData(sFrmData.FrameIndex);
                    if (dt == null)
                    {
                        continue;
                    }
                    for (int i = 0; i < sFrmData.Gids.Count; i++)
                    {
                        var gid = sFrmData.Gids[i];
                        var pl = LogicMatch.instance.GetPlayer(gid);
                        var inputData = dt.InputData[pl.slot];
                        inputData.input = (EInputEnum)sFrmData.Inputs[i];
                        inputData.inputMoveAngle._serializedValue = sFrmData.InputAngles[i];
                        dt.InputData[pl.slot] = inputData;
                    }
                }
            }

            FrameManager.instance.UpdateFrameData();
            SendFrameData();
        }

        public void SendFrameData()
        {
            C2SFrameUpdate frmUpdate = new C2SFrameUpdate();

            var requireStart = Math.Min(curClientFrame + 1, curServerFrame);
            var requireEnd = curServerFrame;
            if (!FrameManager.instance.frameDataInputs.ContainsKey(requireStart))
            {
                for (; requireEnd <= curServerFrame; requireEnd++)
                {
                    if (FrameManager.instance.frameDataInputs.ContainsKey(requireEnd))
                    {
                        requireEnd -= 1;
                        break;
                    }
                }
            }
            else
            {
                requireStart = -1;
                requireEnd = -1;
            }

            frmUpdate.Start = requireStart;
            frmUpdate.End = requireEnd;

            frmUpdate.Input = (int)InputManager.instance.inputData.input;
            frmUpdate.Angle = InputManager.instance.inputData.inputMoveAngle._serializedValue;
            frmUpdate.Index = ++sendIndex;
            ClientManager.instance.UDPSend(frmUpdate);
        }
    }
}