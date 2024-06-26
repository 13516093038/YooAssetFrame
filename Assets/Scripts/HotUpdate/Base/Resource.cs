using System;
using EggCard;
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
        
        public AssetHandle LoadAsset<T>(string path) where T : Object
        {
            AssetHandle handle = package.LoadAssetAsync<T>(path);
            return handle;
        }
    }
}