using UnityEngine;

namespace HotUpdate
{
    public class IdleState: CharacterStateBase
    {
        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (Input.GetKeyDown(KeyCode.A))
            {
                ChangeState<WalkState>();
            }
        }
    }
}