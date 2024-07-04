using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace HotUpdate
{
    public class FsmBase<T> where T : StateBase<T>
    {
        protected T currentState;
        protected List<T> stateList;

        public virtual void Start<TK>() where TK : T
        {
            currentState = stateList.Find(x=> x.GetType() == typeof(TK));
            currentState.OnEnter();
        }

        public virtual void Start(Type stateType) 
        {
            currentState = stateList.Find(x=> x.GetType() == stateType);
            currentState.OnEnter();
        }
        
        public T GetCurrentState()
        {
            return currentState;
        }

        public virtual void ChangeState<TK>() where TK : T
        {
            if (currentState == null)
            {
                Debug.LogWarning("请先初始化状态机");
                return;
            }

            if (currentState.GetType() == typeof(TK))
            {
                Debug.LogWarning("当前正处于此状态");
                return;
            }
            
            currentState.OnLeave();
            currentState =  stateList.Find(x=> x.GetType() == typeof(TK));
            currentState.OnEnter();
        }

        public virtual void Update()
        {
            currentState?.OnUpdate();
        }

        public virtual void InitFsm(params T[] states)
        {
            stateList = new List<T>(states);
            foreach (var state in states)
            {
                state.SetFsm(this);
            }
        }

        public virtual void Destroy()
        {
            stateList.Clear();
        }
    }
}