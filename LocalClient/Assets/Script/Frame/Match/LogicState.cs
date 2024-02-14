using CenterBase;
using UnityEngine;

namespace Game
{
    public enum ELogicType
    {
        none = 0,
        Login,
        Match
    }
    
    public class LogicFSM : FSM<LogicState>
    {
    
        
        public LogicFSM()
        {
            
            AddState(new LoginState(this));
            AddState(new MatchingState(this));
            ChangeState(ELogicType.Login);
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
                    _btnStyle.fontSize = 60;
                    //_btnStyle.fixedWidth = 120;
                    _btnStyle.fixedWidth = 600;
                }

                return _btnStyle;
            }
        }

    
        protected LogicState(ELogicType stType, LogicFSM fsm) : base((int)stType, fsm)
        {
        }
        
        public virtual void OnGUIUpdate(){}
    }

}
