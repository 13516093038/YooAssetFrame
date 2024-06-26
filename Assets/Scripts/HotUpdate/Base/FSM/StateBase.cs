using System.Net;

namespace HotUpdate
{
    public abstract class StateBase<T> : IState where T : StateBase<T>
    {
        private FsmBase<T> Fsm; 
        public virtual void OnEnter()
        {
            
        }

        public virtual void OnFixedUpdate()
        {
            
        }

        public virtual void OnUpdate(){
            
        }
        
        
        public virtual void OnLeave()
        {

        }

        public virtual string GetStateName()
        {
            return this.GetType().Name;
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