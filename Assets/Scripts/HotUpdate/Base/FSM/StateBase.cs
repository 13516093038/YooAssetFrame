using System.Net;
using UnityEngine;

namespace HotUpdate
{
    public class StateBase<T> : IState where T : StateBase<T>
    {
        private FsmBase<T> Fsm; 
        public virtual void OnEnter()
        {
            Debug.Log( GetStateName() + " OnEnter");
        }

        public virtual void OnFixedUpdate()
        {
            
        }

        public virtual void OnUpdate(){
            
        }
        
        
        public virtual void OnLeave()
        {
            Debug.Log( GetStateName() + " OnLeave");
        }
        
        public virtual string GetStateName()
        {
            return GetType().Name;
        }

        protected virtual void ChangeState<TK>() where TK : T
        {
            Fsm.ChangeState<TK>();
        }

        public virtual void SetFsm(FsmBase<T> fsm)
        {
            Fsm = fsm;
        }
    }
}