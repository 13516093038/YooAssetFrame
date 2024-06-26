using UnityEngine;

namespace HotUpdate
{
    public interface IState
    {
        void OnEnter();
        public void OnFixedUpdate();
        public void OnUpdate();
        void OnLeave();
    }
}