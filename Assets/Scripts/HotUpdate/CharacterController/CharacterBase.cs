using System;
using UnityEngine;

namespace HotUpdate
{
    public class CharacterBase : MonoBehaviour
    {
        //基础移动速度
        protected float _speedBase;
        //生命值
        protected float _health;
        //攻击力
        protected float _aggressivity;
        //防御力
        protected float _defensive;
        //护甲穿透率
        protected float _armorPenetrationRate;
        //暴击率
        protected float _criticalHitRate;
        //暴击伤害
        protected float _criticalDamageMultiplier;
        //动画控制器
        protected Animator _animator;
        //状态机
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

        protected void OnDisable()
        {
            _characterFSM.Destroy();
        }

        protected void OnDestroy()
        {
            _characterFSM.Destroy();
        }
    }
}