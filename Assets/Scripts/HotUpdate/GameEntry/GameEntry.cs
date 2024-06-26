using UnityEngine;
using YooAsset;
using Object = UnityEngine.Object;

namespace HotUpdate
{
    public class GameEntry
    {
        public static void LoadGameRoot()
        {
            Resource.Ins.LoadAsset<GameObject>("Assets/Prefabs/GameRoot.prefab", (obj) =>
            {
                Object.Instantiate(obj);
            });
        }
    }
}
