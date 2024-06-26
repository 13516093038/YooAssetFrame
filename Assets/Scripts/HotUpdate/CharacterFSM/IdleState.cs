using UnityEngine;

namespace HotUpdate
{
    public class IdleState: CharacterStateBase
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("IdleState OnEnter");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            Debug.Log("Asdasd");
            if (Input.GetKeyDown(KeyCode.A))
            {
                ChangeState<WalkState>();
            }
        }
    }
}