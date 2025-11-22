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

    public struct MatchPlayerInfo
    {
        public int guid;
        public string name;
    }

    public struct MatchInfo
    {
        public MatchPlayerInfo[] players;
    }
    public class MatchAgent:BaseAgent
    {
        public static int matchIndex;
        public int matchGuid { get;private set; }
        HashSet<int> loadingPlayers = new HashSet<int>();
        public MatchState state { private set; get; }
        private List<PlayerFrame> _frameInputs = new List<PlayerFrame>(10000);
        private int curFrame => _frameInputs.Count -1;
        
        public StreamWriter videoWriter;
        public StreamWriter logWriter;
        public MatchInfo matchInfo;
        
        public MatchAgent(MatchInfo matchInfo)
        {
            this.matchInfo = matchInfo;
            state = MatchState.Idle;
        }
        public void StartMatch(int matchId)
        {
            loadingPlayers.Clear();
            _frameInputs.Clear();
            matchGuid = ++matchIndex;
            state = MatchState.Waiting;
            foreach (var plInfo in this.matchInfo.players)
            {
                var pl = PlayerManager.instance.GetPlayerByGuid(plInfo.guid);
                if (pl == null )
                {
                    continue;
                }
                MatchPlayerInfo info = new MatchPlayerInfo();
                info.guid = pl.guid;
                info.name = pl.name;
                loadingPlayers.Add(info.guid);
            }
            
            string savePath = Directory.GetCurrentDirectory() + "\\..\\SaveData";
            Directory.CreateDirectory(savePath);
            
            var videoPath = savePath + $"\\video_{matchGuid}.bytes";
            if (videoWriter!=null)
                videoWriter.Close();
            if (Directory.Exists(videoPath))
            {
                File.Delete(videoPath);
                File.Create(videoPath).Dispose();
            }
                    
                    
            videoWriter = new StreamWriter(videoPath);

            string logPath = savePath + $"\\..\\SaveData\\log_{matchGuid}.txt";
            if (logWriter!=null)
            {
                logWriter.Close();   
            }
            File.Delete(logPath);
            File.Create(logPath).Dispose();
            logWriter = new StreamWriter(logPath);

            var s2cMatchInfo = GetMatchInfo();

            foreach (var plInfo in matchInfo.players)
            {
                SendPlayerTCP(plInfo.guid, EMessage.S2CStartMatch, s2cMatchInfo);
            }
            Console.Write($"Start Match{matchId}\n");
        }

        public S2CMatchInfo GetMatchInfo()
        {
            var s2cMatchInfo = new S2CMatchInfo();
            s2cMatchInfo.RoomGuid = s2cMatchInfo.RoomGuid;
            for (int i = 0; i < matchInfo.players.Length; i++)
            {
                var plInfo = matchInfo.players[i];
                s2cMatchInfo.Players.Add(new S2CPlayerData(){Guid = plInfo.guid, Name = plInfo.name});
            }
            s2cMatchInfo.Pot = ServerLogic.udpPot;
            var random = new Random();
            s2cMatchInfo.RandomSeed = random.Next(int.MinValue,int.MaxValue);
            s2cMatchInfo.MatchGuid = matchGuid;
            return s2cMatchInfo;
        }
        
        public void EndMatch()
        {
            state = MatchState.Idle;
            logWriter.Flush();
            logWriter.Close();
            videoWriter.Flush();
            videoWriter.Close();
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
                    foreach (var plInfo in matchInfo.players)
                    {
                        ServerLogic.SendUDP(plInfo.guid,frmDt);
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

            for (int i = 0; i < matchInfo.players.Length; i++)
            {
                var emGid = matchInfo.players[i].guid;
                int input = 0;
                long angle = 0;
                for (int pi = 0; pi < frmUpdate.Gids.Count; pi++)
                {
                    if (emGid == frmUpdate.Gids[pi])
                    {
                        input = frmUpdate.Inputs[pi];
                        angle = frmUpdate.InputAngles[pi];
                    }
                }
                logWriter.WriteLine($"{i}:: input:{input} angle:{angle}");
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
                int end = Math.Min(upData.Start + 500, upData.End);
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

        public void SetMatchInfo(S2CMatchInfo matchInfo)
        {
            matchInfo.Players.Clear();
            foreach (var pl in this.matchInfo.players)
            {
                var s2cPl = new S2CPlayerData();
                s2cPl.Guid = pl.guid;
                s2cPl.Name = pl.name;
                matchInfo.Players.Add(s2cPl);
                
            }
        }


        public void ReEnterMatch(PlayerAgent player)
        {
            SendPlayerTCP(player.guid, EMessage.S2CStartMatch, GetMatchInfo());
        }
    }    
}

