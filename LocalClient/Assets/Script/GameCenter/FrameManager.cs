using System;
using System.Collections.Generic;
using C2SProtoInterface;
using CenterBase;
using TrueSync;
using UnityEngine;


namespace Game
{
    public struct originalData
    {
        public TSVector orPos;
        public TSQuaternion orRotation;
    }

    public struct InputData
    {
        public FP inputMoveAngle;
        public EInputEnum input;

        public void Clear()
        {
            inputMoveAngle = -1;
            input = EInputEnum.none;
        }

        public InputData(long iAngle, int iInput)
        {
            inputMoveAngle = FP.FromRaw(iAngle);
            input = (EInputEnum)iInput;
        }
    }

    public static class ExProto
    {
        public static InputData GetInputData(this S2CFrameData frmInputData, int idx)
        {
            return new InputData { input = (EInputEnum)frmInputData.Inputs[idx], inputMoveAngle = FP.FromRaw(frmInputData.InputAngles[idx]) };
        }
    }

    public class FrameManager : BasicMonoSingle<FrameManager>
    {
        public FP frameTime = 0.02f;
        public GameType gameType = GameType.Play;
        [SerializeField] private Transform players;

        private originalData lorData = new originalData() { orPos = TSVector.forward, orRotation = default };
        private originalData rOrData = new originalData() { orPos = TSVector.back, orRotation = default };
        private Dictionary<int, S2CFrameData> frameDataInputs = new Dictionary<int, S2CFrameData>();
        public CharacterModel[] characters;
        private float timeCount;
        public int curServerFrame = -1;
        public int curClientFrame = -1;
        public int playerNum;

        public override void OnAwake()
        {
            base.OnAwake();
            characters = players.GetComponentsInChildren<CharacterModel>(true);
        }

        /// <summary>
        /// 重新开始前设置一下
        /// </summary>
        public void ResetPlay(int plNum)
        {
            playerNum = plNum;
            gameType = GameType.Play;
            ResetCharacters();
            frameDataInputs.Clear();
            curClientFrame = -1;
        }

        public void UpdateInputData()
        {
            switch (gameType)
            {
                case GameType.Play:
                    SetFrameDatas();
                    PlayFrames();
                    break;
                    // case GameType.TraceFrame:
                    //     frameDataInputs.Insert(frameDataInputs.Count, proto);
                    //     break;
                    // case GameType.PlayBack:
                    //     UpdatePlayers();
                    //     curClientFrame = proto.frameIndex;
                    break;
            }
        }

        private void Update()
        {
            switch (gameType)
            {
                case GameType.PlayBack:
                    UpdatePlayBack();
                    break;
            }
        }


        private void UpdatePlayBack()
        {
            timeCount += Time.deltaTime;
            int curIndex = (int)Mathf.Floor(timeCount / (float)frameTime);
            while (curIndex < frameDataInputs.Count && curClientFrame < curIndex)
            {
                curClientFrame = curIndex;

                // if (curClientFrame >= 0)
                //     UpdateInputData( frameDataInputs[curIndex] );
            }
        }

        public void PlayBack()
        {
            // frameDataInputs.Clear();
            // frameDataInputs.AddRange(ClientManager.instance.inputs.ToArray());
            // gameType = GameType.PlayBack;
            // ResetCharacters();
            //
            // timeCount = 0;
            // curClientFrame = 0;
        }

        void ResetCharacters()
        {
            foreach (var character in characters)
            {
                if (character.isLeftTeam)
                {
                    character.tsTrans.position = lorData.orPos;
                    character.tsTrans.rotation = lorData.orRotation;
                }
                else
                {
                    character.tsTrans.position = rOrData.orPos;
                    character.tsTrans.rotation = rOrData.orRotation;
                }
            }
        }

        public void Pause()
        {
            gameType = GameType.Pause;
        }

        public void Play()
        {
            // gameType = GameType.Play;
            // curClientFrame = -1;
            // ClientManager.instance.Play();
        }


        private int tracingFrameIndex;

        public void Continue()
        {
            gameType = GameType.Play;
        }

        private void SetFrameDatas()
        {
            S2CFrameUpdate frmServerData = ClientManager.instance.UDPReceive();
            if (frmServerData != null)
            {
                curServerFrame = Math.Max(curServerFrame, frmServerData.CurServerFrame);
                foreach (var frmDt in frmServerData.FrameDatas)
                {
                    Debug.Log($"插入帧 {frmDt.FrameIndex} 输入角度 {frmDt.InputAngles[0]} {DateTime.Now}");
                    frameDataInputs[frmDt.FrameIndex] = frmDt;
                }
            }

            C2SFrameUpdate frmUpdate = new C2SFrameUpdate();

            var requireStart = Math.Min(curClientFrame + 1, curServerFrame);
            var requireEnd = curServerFrame;
            if (!frameDataInputs.ContainsKey(requireStart))
            {
                for (; requireEnd <= curServerFrame; requireEnd++)
                {
                    if (frameDataInputs.ContainsKey(requireEnd))
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

            frmUpdate.Angle = InputManager.instance.inputData.inputMoveAngle._serializedValue;
            InputManager.instance.inputData.Clear();
            ClientManager.instance.UDPSend(frmUpdate);
        }

        private void PlayFrames()
        {
            if (frameDataInputs.TryGetValue(curClientFrame + 1, out var curFrame))
            {
                curClientFrame++;
                for (int i = 0; i < playerNum; i++)
                {
                    characters[i].UpdateInput(curFrame.GetInputData(i));
                }
            }
        }
    }
}