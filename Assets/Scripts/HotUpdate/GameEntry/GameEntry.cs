using UnityEngine;

namespace HotUpdate
{
    public class GameEntry
    {
        public static void LoadGameRoot()
        {
            Resource.Ins.LoadAsset<GameObject>("Assets/Prefabs/GameRoot.prefab", (obj) =>
            {
                GameObject.Instantiate(obj);
                Debug.Log("GameRoot加载完毕");
            });
        }
    }
}
