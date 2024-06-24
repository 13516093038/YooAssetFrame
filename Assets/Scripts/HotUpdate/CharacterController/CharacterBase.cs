using System;
using UnityEngine;

namespace HotUpdate
{
    public class CharacterBase : MonoBehaviour
    {
        private Animator _animator;
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }
    }
}