using System;
using System.Collections.Generic;
using EggCard;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace HotUpdate
{
    public class PoolMgr : MonoSingleton<PoolMgr>
    {
        [Header("Is Auto Release Default")] [SerializeField]
        private bool _isAutoRelease;

        [Header("Expire Time")] [SerializeField]
        private int _expireTime;

        [Header("Default Capacity")] [SerializeField]
        private int _capacity;

        private Dictionary<string, object> gamePools = new();

        public GamePool<T> CreatePool<T>(string path, Transform parent = null) where T : Object
        {
            if (gamePools.TryGetValue(path, out var gamePool))
            {
                Debug.LogWarning("Pool already exists");
                return (GamePool<T>) gamePool;
            }
            GamePool<T> pool = new GamePool<T>(path, parent);
            gamePools.Add(path, pool);
            return pool;
        }

        public void DestroyPool(string path)
        {
            if (!gamePools.ContainsKey(path))
            {
                Debug.LogWarning("Pool not exists");
            }
            else
            {
                gamePools.Remove(path);
            }
        }

        public void DestroyAllPool()
        {
            gamePools.Clear();
        }
        
        public class GamePool<T> where T : Object
        {
            private List<GameObject> gameObjs;
            private string path;
            private Transform parent;
            private PoolMgr poolMgr;
            
            public GamePool(string path, Transform parent = null)
            {
                poolMgr = Ins;
                gameObjs = new List<GameObject>();
                this.parent = new GameObject(typeof(T).Name).transform;
                this.parent.SetParent(poolMgr.transform);
                this.path = path;
            }

            private void AddToGameObjs(GameObject obj)
            {
                obj.SetActive(false);
                gameObjs.Add(obj);
                if (poolMgr._isAutoRelease)
                {
                    MonoMgr.Ins.StartDelayCall(poolMgr._expireTime, () =>
                    {
                        Destroy(obj);
                    });
                }
            }

            public void TakeOut(Action<T> callback) 
            {
                if (gameObjs.Count > 0)
                {
                    GameObject obj = gameObjs[0];
                    gameObjs.Remove(obj);
                    obj.gameObject.SetActive(true);
                    obj.transform.SetParent(poolMgr.transform);
                    callback?.Invoke(obj as T);
                }
                else
                {
                    Resource.Ins.LoadAsset<T>(path, (obj) =>
                    {
                        GameObject go = Instantiate(obj as GameObject, parent);
                        go.transform.localPosition = Vector3.zero;
                        go.transform.localRotation = Quaternion.identity;
                        go.transform.localScale = Vector3.one;
                        go.SetActive(true);
                        callback?.Invoke(obj);
                    });
                }
            }

            public void Recovery(T obj)
            {
                if(poolMgr._capacity > gameObjs.Count)
                {
                    GameObject go = obj as GameObject;
                    go.transform.parent = parent;
                    AddToGameObjs(go);
                }
                else
                {
                    Destroy(obj);
                }
            }
        }
    }
}