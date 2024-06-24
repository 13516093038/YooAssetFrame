using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using YooAsset;
using Object = UnityEngine.Object;

namespace YooAssetFrame
{
    public class GameEntry
    {
        private static ResourcePackage package;
        
        public static void LoadGameRoot()
        {
            // 初始化资源系统
            YooAssets.Initialize();
            // 创建默认的资源包
            package = YooAssets.TryGetPackage("DefaultPackage");

            LoadAsset<GameObject>("Assets/Prefabs/GameRoot.prefab", (obj) =>
            {
                Object.Instantiate(obj);
            });
        }
        
        public static async void LoadAsset<T>(string path, Action<T> callback) where T : Object
        {
            AssetHandle handle = package.LoadAssetAsync<T>(path);
            await handle.Task;
            T t = handle.AssetObject as T;
            callback(t);
        }
        
        public static AssetHandle LoadAsset<T>(string path) where T : Object
        {
            AssetHandle handle = package.LoadAssetAsync<T>(path);
            return handle;
        }
    }
}
