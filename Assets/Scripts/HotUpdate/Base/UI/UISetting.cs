using UnityEngine;

namespace HotUpdate
{
    [CreateAssetMenu(fileName = "UISetting", menuName = "UISetting", order = 0)]
    public class UISetting : ScriptableObject
    {
        //是否使用单遮模式
        public bool SINGMASK_SYSTEM;
    }
}