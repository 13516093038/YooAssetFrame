using System;
using System.Collections;
using EggCard;
using UnityEngine;

namespace HotUpdate
{
    public class MonoMgr : MonoSingleton<MonoMgr>
    {
        public void StartDelayCall(float time, Action callback)
        {
            StartCoroutine(DelayCall(time, callback));
        }
        
        IEnumerator DelayCall(float time, Action callback)
        {
            yield return new WaitForSeconds(time);
            callback();
        }
    }
}