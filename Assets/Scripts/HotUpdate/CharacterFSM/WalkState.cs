using UnityEngine;

namespace HotUpdate
{
    public class WalkState : CharacterStateBase
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("WalkState OnEnter");
        }
        
    }
}