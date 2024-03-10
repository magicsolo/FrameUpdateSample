using System.Diagnostics.CodeAnalysis;
using CenterBase;
using TrueSync;
using TrueSync.Physics2D;
using UnityEngine;

namespace Game
{
    public enum EPlayerState
    {
        none = 0,
        Idle,
        Move,
        Total,
        Attack,
    }

    public class PlayerFSM : FSM<PS_Base>
    {
        public LogicPlayer owner;
        public PlayerFSM(LogicPlayer owner)
        {
            this.owner = owner;
            AddState(new PS_Idle(this));
            AddState(new PS_Move(this));
            AddState(new PS_Attack(this));
            ChangeState(EPlayerState.Idle);
        }
        public bool ChangeState(EPlayerState stateType,object param = null)
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
        protected InputData input => owner.playerData.InputData;
        public LogicPlayer owner => plfsm.owner;
        public EPlayerState curState => (EPlayerState)stateType;
        protected bool _finished;
        private int _enterTime;
        private int _totalTime;
        private object _nxtStateParam;
        protected EPlayerState _nxtStateType;
        private int passedTime => FrameManager.instance.curTime - _enterTime;
        private int _fireFrame;
        private LogicAnimInfo _animLogicInfo;
        protected TSVector2 Pos;
        protected FP area;
        public PS_Base(EPlayerState stateType, FSM<PS_Base> fsm) : base((int)stateType, fsm)
        {
            this.plfsm = (PlayerFSM)fsm;
        }

        public override void Enter(FSMState<PS_Base> lstState, object param = null)
        {
            _nxtStateType = 0;
            _finished = false;
            _enterTime = FrameManager.instance.curTime;
            
            base.Enter(lstState, param);
        }
        
        protected void PlayState(string animName)
        {
            var asset = AssetLogicAnimAinfosManager.instance.GetAnimInfo(animName);
            _totalTime = asset.length;
            owner.playerData.aniInfo.stateName = asset.stateName;
            owner.playerData.aniInfo.totalTime = _totalTime;
            owner.playerData.aniInfo.startTime = FrameManager.instance.curTime;
            _fireFrame = asset.fireFrame;
            Pos = asset.skillCheckArea.xy;
        }

        public override void Update()
        {
            base.Update();
            LogicUpdate();
            InputCheck();
            owner.playerData.aniInfo.curTime = FrameManager.instance.curTime - _enterTime;
            if (passedTime == _fireFrame)
                OnFire();
            
            if (passedTime > _totalTime)
                _finished = true;
            if (_finished)
            {
                if (_nxtStateType != 0 && plfsm.ChangeState(_nxtStateType, _nxtStateParam))
                {
                    _nxtStateType = 0;
                }
                else
                {
                    if (owner.playerData.InputData.inputMoveAngle > -1)
                    {
                        plfsm.ChangeState(EPlayerState.Move);
                    }
                    else
                    {
                        plfsm.ChangeState(EPlayerState.Idle);
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