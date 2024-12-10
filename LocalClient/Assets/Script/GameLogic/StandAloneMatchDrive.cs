using FrameDrive;

namespace Game
{
    public class StandAloneMatchDrive:BaseMatchDrive
    {
        private int _spaceTime;
        protected override int spaceTime => _spaceTime;
        
        public void Start(PlayerFiled[] playerFileds)
        {
            _spaceTime = (FrameManager.frameTime*1000).AsInt();
            StartDrive(playerFileds);
        }

        protected override void Update()
        {
            var nxtFrame = FrameManager.instance.AddFrameData(FrameManager.instance.curClientFrame + 1);
            nxtFrame.InputData[0].input = InputManager.instance.inputData.input;
            nxtFrame.InputData[0].inputMoveAngle = InputManager.instance.inputData.inputMoveAngle;
            FrameManager.instance.UpdateFrameData();
        }
        
        
    }
}