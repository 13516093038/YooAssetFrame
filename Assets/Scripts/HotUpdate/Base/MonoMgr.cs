using System;
using System.Collections;
using System.Collections.Generic;
using Codice.Client.Common;
using EggCard;
using UnityEngine;
using Time = UnityEngine.Time;

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