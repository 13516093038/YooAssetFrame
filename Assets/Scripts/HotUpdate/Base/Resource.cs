using System;
using System.Threading.Tasks;
using EggCard;
using UnityEngine;
using YooAsset;
using Object = UnityEngine.Object;

namespace HotUpdate
{
    public class Resource : Singleton<Resource>
    {
        private ResourcePackage package;
        public Resource()
        {
            // 初始化资源系统
            YooAssets.Initialize();
            // 创建默认的资源包
            package = YooAssets.TryGetPackage("DefaultPackage");
        }
        
        public async void LoadAsset<T>(string path, Action<T> callback) where T : Object
        {
            AssetHandle handle = package.LoadAssetAsync<T>(path);
            await handle.Task;
            T t = handle.AssetObject as T;
            callback(t);
        }
        
        public async Task<T> LoadAsset<T>(string path) where T : Object
        {
            AssetHandle handle = package.LoadAssetAsync<T>(path);
            await handle.Task;
            return handle.AssetObject as T;
        }

        public async Task<GameObject> LoadAsset(string path)
        {
            AssetHandle handle = package.LoadAssetAsync<GameObject>(path);
            await handle.Task;
            return handle.AssetObject as GameObject;
        }
    }
}