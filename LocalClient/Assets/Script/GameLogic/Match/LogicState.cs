using CenterBase;
using UnityEngine;

namespace Game
{
    public enum ELogicType
    {
        none = 0,
        Disconnection,
        Match,
        Video,
        StandAloneRoom,
        StandAloneMatching,
    }
    
    public class LogicFSM : FSM<LogicState>
    {
    
        
        public LogicFSM()
        {
            AddState(new DisconnectionState(this));
            AddState(new MatchingState(this));
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

    
        protected LogicState(ELogicType stType, LogicFSM fsm) : base((int)stType, fsm)
        {
        }
        
        public virtual void OnGUIUpdate(){}
    }

}
