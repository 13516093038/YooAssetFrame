using System;

namespace HotUpdate
{
    public class Mega : CharacterBase
    {
        protected override void Awake()
        {
            base.Awake();
            _characterFSM.InitFsm(new IdleState(), new WalkState());
            _characterFSM.Start<IdleState>();
        }
    }
}