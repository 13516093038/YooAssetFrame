using System;
using UnityEngine;

namespace HotUpdate
{
    public class CharacterBase : MonoBehaviour
    {
        protected Animator _animator;
        protected CharacterFSM _characterFSM; 
        protected virtual void Awake()
        {
            _characterFSM = new CharacterFSM();
            _animator = GetComponent<Animator>();
        }

        protected virtual void Update()
        {
            _characterFSM.Update();
        }
    }
}