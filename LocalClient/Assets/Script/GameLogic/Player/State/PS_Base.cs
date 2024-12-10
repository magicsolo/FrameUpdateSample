using CenterBase;
using FrameDrive;
using TrueSync;
using InputData = TrueSync.InputData;

namespace Game
{
    public enum EPlayerState
    {
        none = 0,
        Idle,
        Move,
        Attack,
        Hurt,
        Total,
        
    }

    public class PlayerFSM : FSM<PS_Base>
    {
        public LogicPlayer owner;
        private EPlayerState _nextState;
        public EPlayerState nextState => _nextState;
        private object _nextSateParam;
        public PlayerFSM(LogicPlayer owner)
        {
            this.owner = owner;
            AddState(new PS_Idle(this));
            AddState(new PS_Move(this));
            AddState(new PS_Attack(this));
            AddState(new PS_Hurt(this));
            ChangeState(EPlayerState.Idle);
        }

        public void Update()
        {
            if (_nextState!= EPlayerState.none)
            {
                var nxtState = _nextState;
                ChangeState(nxtState, _nextSateParam);
                _nextState = EPlayerState.none;
                _nextSateParam = null;
            }
            
            if (curState!=null)
                curState.Update();
        }

        public void SetNextState(EPlayerState nxtState,object param = null)
        {
            _nextState = nxtState;
            _nextSateParam = param;
        }
        
        private bool ChangeState(EPlayerState stateType,object param = null)
        {
             return ChgST((int)stateType,param);
        }

        public PS_Base GetStateByType(EPlayerState stateType)
        {
            return GetState(((int)stateType));
        }
        
    }
    public abstract class PS_Base:FSMState<PS_Base>
    {
        protected PlayerFSM plfsm;
        protected FrameInputData FrameInput => owner.filed.data.inputData;
        public LogicPlayer owner => plfsm.owner;
        public EPlayerState curState => (EPlayerState)stateType;
        protected bool _finished;
        private object _nxtStateParam;
        protected EPlayerState _nxtStateType;
        private int _fireFrame;
        private LogicAnimInfo _animLogicInfo;
        protected bool useAnimDrive;
        protected TSVector2 _curSkillPos => _animLogicInfo.GetSkillPos(passedFrame);
        protected TSVector _curSkillArea => _animLogicInfo.GetSkillArea(passedFrame);
        protected TSVector _curAnimPos => _animLogicInfo.GetPos(passedFrame);
        protected TSVector _enterPos;

        public int enterFrame { get; private set; }
        public int passedFrame => FrameManager.instance.clientRuningFrame - enterFrame;
        public int totalFrame => _animLogicInfo.length;
        public PS_Base(EPlayerState stateType, FSM<PS_Base> fsm) : base((int)stateType, fsm)
        {
            this.plfsm = (PlayerFSM)fsm;
        }

        public override void Enter(FSMState<PS_Base> lstState, object param = null)
        {
            _enterPos = owner.filed.data.pos;
            _animLogicInfo = default;
            _nxtStateType = 0;
            _finished = false;
            enterFrame = FrameManager.instance.clientRuningFrame;
            
            base.Enter(lstState, param);
        }
        
        protected void PlayState(string animName)
        {
            var asset = AssetLogicAnimAinfosManager.instance.GetAnimInfo(animName);
            _animLogicInfo = asset;
            owner.filed.data.aniInfo.stateName = asset.stateName;
            owner.filed.data.aniInfo.totalFrame = asset.length;
            owner.filed.data.aniInfo.startFrame = FrameManager.instance.curTime.AsInt();
            _fireFrame = asset.fireFrame;
        }

        public override void Update()
        {
            base.Update();
            if (useAnimDrive)
            {
                var animPos = _curAnimPos;
                animPos.x *= owner.filed.data.faceRight ? 1 : -1;
                owner.filed.data.pos = _enterPos + animPos;
            }
            
            LogicUpdate();
            InputCheck();
            owner.filed.data.aniInfo.curFrame = passedFrame;
            if (passedFrame == _fireFrame )
                OnFire();
            
            if (passedFrame >= totalFrame)
                _finished = true;
            if (_finished)
            {
                if (plfsm.nextState== EPlayerState.none)
                {
                    if (_nxtStateType != 0)
                    {
                        plfsm.SetNextState(_nxtStateType, _nxtStateParam);
                        _nxtStateType = 0;
                    }
                    else
                    {
                        if (owner.filed.data.inputData.inputMoveAngle >= 0)
                        {
                            plfsm.SetNextState(EPlayerState.Move);
                        }
                        else
                        {
                            plfsm.SetNextState(EPlayerState.Idle);
                        }
                    }
                }
            }
        }
        
        public virtual void OnFire(){}

        protected virtual void LogicUpdate(){}

        public virtual void InputCheck()
        {
            
        }
    }
}