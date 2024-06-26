using System;
using System.Collections.Generic;
using EggCard;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HotUpdate
{
    public class PoolMgr : MonoSingleton<PoolMgr>
    { 
        [Header("Is Auto Release")] 
        [SerializeField] private bool _isAutoRelease; 
        [Header("Expire Time")] 
        [SerializeField] private int _lifeTime;
        [Header("Capacity")] 
        [SerializeField] private int _capacity;

        private Dictionary<string, List<GameObject>> _gamePool = new();
        private Dictionary<string, Transform> _objParents = new();

        public async void TakeOut<T>(string path, Action<T> callback = null, Transform parent = null) where T : Transform
        {
            //如果是首个对象
            if(!_gamePool.ContainsKey(path))
            {
                _gamePool.Add(path, new List<GameObject>());
                Action<T> loadSuccess = (obj) =>
                {
                    Instantiate(obj, parent);
                    obj.position = Vector3.zero;
                };
                loadSuccess += callback;
                Resource.Ins.LoadAsset<T>(path, loadSuccess);
            }
            
        }

        public void Recovery<T>(string path, Transform obj) where T : Object
        {
            
        }
    }
}