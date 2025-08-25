using FrameDrive;

namespace Game
{
    public class StandAloneMatchDrive:BaseMatchDrive
    {
        private int _spaceTime;
        protected override int spaceTime => _spaceTime;
        
        public void Start(MatchInfo matchInfo)
        {
            _spaceTime = (FrameManager.frameTime*1000).AsInt();
            StartDrive(matchInfo);
        }

        protected override void OnUpdate()
        {
            var nxtFrame = FrameManager.instance.AddFrameData(FrameManager.instance.clientRuningFrame + 1);
            nxtFrame.InputData[0].input = InputManager.instance.inputData.input;
            nxtFrame.InputData[0].inputMoveAngle = InputManager.instance.inputData.inputMoveAngle;
            FrameManager.instance.UpdateFrameData();
        }
        
        
    }
}