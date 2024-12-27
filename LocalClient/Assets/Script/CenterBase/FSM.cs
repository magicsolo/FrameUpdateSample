using System.Collections.Generic;
using JetBrains.Annotations;

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

        protected bool ChgST(int stType,object param = null)
        {
            bool succ = false;
            if (states.TryGetValue(stType,out  var st) && (curState == null || curState.CanEnterState(st)) && st.CanEnter() )
            {
                if (curState!=null)
                    curState.Exit();

                curState = st;
                st.BeforeEnter();
                st.Enter(curState,param);
                succ = true;
            }

            return succ;
        }

        [CanBeNull]
        protected T GetState(int stType)
        {
            states.TryGetValue(stType, out var st);
            return st;
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
        public virtual void BeforeEnter(){}
        public virtual void Enter(FSMState<T> lstState,object param = null){}

        //下一个状态
        public virtual bool CanEnterState(T newSt)
        {
            return true;
        }

        public virtual bool CanEnter()
        {
            return true;
        }
    }
}