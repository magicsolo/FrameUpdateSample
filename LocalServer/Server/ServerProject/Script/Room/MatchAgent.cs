using C2SProtoInterface;
using Google.Protobuf;

namespace GameServer
{
    public enum MatchState
    {
        Idle,
        Waiting,
        Matching,
    }
    public class MatchAgent
    {
        public static int matchIndex;
        private int matchGuid;
        HashSet<int> loadingPlayers = new HashSet<int>();
        List<int> players = new List<int>();
        MatchState state = MatchState.Idle;
        private List<PlayerFrame> _frameInputs = new List<PlayerFrame>(10000);
        private int curFrame => _frameInputs.Count -1;
        
        public StreamWriter videoWriter;
        public StreamWriter logWriter;
        
        public MatchAgent()
        {
            state = MatchState.Idle;
        }
        public void StartMatch(List<RommPlayerInfo> allPlayers)
        {
            loadingPlayers.Clear();
            matchGuid = matchIndex++;
            state = MatchState.Waiting;
            players.Clear();
            foreach (var pl in allPlayers)
            {
                players.Add(pl.guid);
                loadingPlayers.Add(pl.guid);
            }
            
            string videoPath = Directory.GetCurrentDirectory() + "\\..\\SaveData";
            Directory.CreateDirectory(videoPath);
            videoPath += $"\\{matchGuid}_video.bytes";
            if (videoWriter!=null)
                videoWriter.Close();
            if (Directory.Exists(videoPath))
            {
                File.Delete(videoPath);
                File.Create(videoPath).Dispose();
            }
                    
                    
            videoWriter = new StreamWriter(videoPath);

            string logPath = Directory.GetCurrentDirectory() + "\\..\\SaveData\\log.txt";
            if (logWriter!=null)
            {
                logWriter.Close();   
            }
            File.Delete(logPath);
            File.Create(logPath).Dispose();
            logWriter = new StreamWriter(logPath);
        }

        public void Update()
        {
            if (state != MatchState.Matching)
            {
                return;
            }
            lock (_frameInputs)
            {
                if (_frameInputs.Count > 0)
                {
                    var frmDt = new S2CFrameUpdate();//ServerFrameManager.GetSendFrameData(curFrame,_frameInputs[_frameInputs.Count - 1]);
                    frmDt.CurServerFrame = curFrame;
                    AddFrameData(frmDt, _frameInputs.Count - 1, _frameInputs.Count - 1);
                    foreach (var guid in players)
                    {
                        ServerLogic.SendUDP(guid,frmDt);
                    }
                    SaveFrameBytes(frmDt);
                }

                PlayerFrame input = new PlayerFrame();
                input.Init(_frameInputs.Count);
                _frameInputs.Add(input);
            }
        }
        
        private void SaveFrameBytes(S2CFrameUpdate frmDt)
        {
            var sendBytes = frmDt.FrameDatas[0].ToByteArray();
            var saveBytes = System.Text.Encoding.UTF8.GetString(sendBytes);
            videoWriter.WriteLine(saveBytes);
            videoWriter.Flush();

            var frmUpdate = frmDt.FrameDatas[0];
            logWriter.WriteLine($"frm:{frmUpdate.FrameIndex}");
            for (int i = 0; i < frmUpdate.Gids.Count; i++)
            {
                var gid = frmUpdate.Gids[i];
                var input = frmUpdate.Inputs[i];
                var angle = frmUpdate.InputAngles[i];
                logWriter.WriteLine($"{gid}:: input:{input} angle:{angle}");
            }
            logWriter.Flush();
        }

        public void ReceiveC2SFrameData(int guid,C2SFrameUpdate upData)
        {
            if (state == MatchState.Idle)
            {
                return;
            }
            if (loadingPlayers.Contains(guid))
            {
                loadingPlayers.Remove(guid);
            }

            if (loadingPlayers.Count > 0)
            {
                return;
            }
            state = MatchState.Matching;

            //丢帧补帧
            int start = Math.Min(upData.Start, curFrame - 1);
            if (start >= 0)
            {
                int end = Math.Max(upData.Start + 500, upData.End);
                end = Math.Min(end, curFrame - 1);
                end = Math.Max(end, start);
                S2CFrameUpdate toC = new S2CFrameUpdate();
                toC.CurServerFrame = curFrame - 1;

                if (end < curFrame)
                    AddFrameData(toC, start, end);

                ServerLogic.SendUDP(guid,toC);
            }

            var idx = -1;
            PlayerFrame frame;
            lock (_frameInputs)
            {
                if (_frameInputs.Count <= 0)
                    return;
                frame = _frameInputs[_frameInputs.Count - 1];

                var inputs = frame.inputs;
                for (int i = 0; i < inputs.Count; i++)
                {
                    if (inputs[i].guid == guid)
                        idx = i;
                }

                PlayerFrameInput curInput;
                if (idx == -1)
                {
                    curInput = new PlayerFrameInput();
                    idx = inputs.Count;
                    curInput.Init(guid);
                    inputs.Add(curInput);
                }
                else
                    curInput = inputs[idx];

                curInput.Refresh(upData);
                inputs[idx] = curInput;
            }
        }
            
        void AddFrameData(S2CFrameUpdate toC, int start, int end)
        {
            start = Math.Max(0, start);
            end = Math.Min(curFrame, end);
            end = Math.Max(start, end);
            for (int sendIdx = start; sendIdx < end + 1; sendIdx++)
            {
                S2CFrameData frmDt = new S2CFrameData();
                frmDt.FrameIndex = sendIdx;
                var frm = _frameInputs[sendIdx];

                for (int i = 0; i < frm.inputs.Count; i++)
                {
                    var input = frm.inputs[i];
                    frmDt.Gids.Add(input.guid);
                    frmDt.Inputs.Add(input.input);
                    frmDt.InputAngles.Add(input.moveAngle);
                }

                toC.FrameDatas.Add(frmDt);
            }
        }

        public void EndMatch()
        {
            state = MatchState.Idle;
        }
    }    
}

