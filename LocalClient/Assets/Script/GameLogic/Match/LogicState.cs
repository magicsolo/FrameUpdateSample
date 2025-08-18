using System;
using C2SProtoInterface;
using CenterBase;
using UnityEngine;

namespace Game
{
    public enum ELogicType
    {
        none = 0,
        Disconnection,
        Lobby,
        Room,
        Match,
        MatchResult,
        Video,
        StandAloneRoom,
        StandAloneMatching,
    }
    
    public class LogicFSM : FSM<LogicState>
    {
    
        
        public LogicFSM()
        {
            AddState(new DisconnectionState(this));
            AddState(new Lobby(this));
            AddState(new Room(this));
            AddState(new MatchingState(this));
            AddState(new MatchResultState(this));
            AddState(new VideoState(this));
            AddState(new StandAloneRoom(this));
            AddState(new StandAloneMatching(this));
        }
        

        public void ChangeState(ELogicType stType,object param = null)
        {
            ChgST((int)stType,param);
        }
    }


    public abstract class LogicState : FSMState<LogicState>
    {
        EventRegister eventRegister = new EventRegister();
        TCPListener tcpListener = new TCPListener();
        protected LogicFSM logicFsm => (LogicFSM)fsm;
        private GUIStyle _btnStyle;

        public GUIStyle btnStyle
        {
            get
            {
                if (_btnStyle == null)
                {
                    _btnStyle = GUI.skin.button;
                    _btnStyle.fontSize = 30;
                    //_btnStyle.fixedWidth = 120;
                    _btnStyle.fixedWidth = 300;
                }

                return _btnStyle;
            }
        }

        private GUIStyle _txtStyle;

        public GUIStyle txtStyle
        {
            get
            {
                if (_txtStyle == null)
                {
                    _txtStyle = GUI.skin.label;
                    _txtStyle.fontSize = 30;
                    _txtStyle.fixedWidth = 300;
                    _txtStyle.normal.textColor = Color.black;
                }

                return _txtStyle;
            }
        }

        protected FSMState<LogicState> lstState;
        protected object param;
    
        protected LogicState(ELogicType stType, LogicFSM fsm) : base((int)stType, fsm)
        {
        }

        public sealed override void Enter(FSMState<LogicState> lstState, object param = null)
        {
            base.Enter(lstState, param);
            this.lstState = lstState;
            this.param = param;
            BeforeEnter();
            OnEnter();
            AfterEnter();
        }

        protected virtual void BeforeEnter()
        {
            
        }
        protected virtual void OnEnter()
        {
            
        }
        
        
        protected virtual void AfterEnter()
        {
            eventRegister.RegistAll();
            tcpListener.RegistAll();
        }

        public override void Exit()
        {
            eventRegister.UnregistAll();
            tcpListener.UnRegistAll();
            base.Exit();
        }


        public virtual void OnGUIUpdate(){}
        
        protected void RegistEvent(string eventName, Action<object> action)
        {
            eventRegister.AddRegister(eventName,action);
        }

        protected void RegistTCPListener(EMessage message, Action<TCPInfo> action)
        {
            tcpListener.AddHandler(message,action);
        }

    }

}
