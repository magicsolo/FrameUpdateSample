using System;
using System.Threading;
using C2SProtoInterface;
using FrameDrive;

namespace Game
{
    public class ServerMatchDrive:BaseMatchDrive
    {
        private S2CStartGame servDt;

        public void Start(S2CStartGame servDt)
        {
            this.servDt = servDt;
            PlayerFiled[] playerFileds = new PlayerFiled[this.servDt.Players.Count];

            for (int i = 0; i < playerFileds.Length; i++)
            {
                var plData = this.servDt.Players[i];
                var plFiled = new PlayerFiled(new PlayerInfo(plData.Guid,plData.Name,i));
                plFiled.data = new PlayerLogicData();
                playerFileds[i] = plFiled;
            }

            StartDrive(playerFileds);
        }
        
        protected override void Update()
        {
            ClientManager.instance.UDPConnect(8091);
            while (true)
            {
                S2CFrameUpdate frmServerData = ClientManager.instance.UDPReceive();

                if (frmServerData?.FrameDatas != null)
                {
                    FrameManager.instance.curServerFrame = frmServerData.CurServerFrame;

                    foreach (var sFrmData in frmServerData.FrameDatas)
                    {
                        FrameData dt = FrameManager.instance.AddFrameData(sFrmData.FrameIndex);

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
                Thread.Sleep(spaceTime);
            }
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
            ClientManager.instance.UDPSend(frmUpdate);
        }
    }
}