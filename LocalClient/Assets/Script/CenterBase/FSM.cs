using System.Collections.Generic;

namespace CenterBase
{
    public abstract class FSM <T> where T:FSMState<T>
    {
        public T curState { get; protected set; }
        private Dictionary<int, T> states = new Dictionary<int, T>();
        
        
        public void AddState(T st )
        {
            if (states.ContainsKey(st.stateType))
                return;
            states.Add(st.stateType,st);
        }

        protected void ChgST(int stType,object param = null)
        {
            if (states.TryGetValue(stType,out  var st) && (curState == null || curState.CanEnterState(st)) )
            {
                if (curState!=null)
                    curState.Exit();

                curState = st;
                st.Enter(curState,param);

            }
        }
    }

    public abstract class FSMState<T> where T:FSMState<T>
    {
        protected FSM<T> fsm;
        public int stateType;

        public FSMState(int stType,FSM<T> fsm)
        {
            this.fsm = fsm;
            stateType = stType;
        }

        public virtual void Update(){}

        public virtual void Exit(){}

        public virtual void Enter(FSMState<T> lstState,object param = null){}

        public bool CanEnterState(T newSt)
        {
            return true;
        }
    }
}