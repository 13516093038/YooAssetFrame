using UnityEngine;

namespace HotUpdate
{
    public static class Utility
    {
        public static string GetWindowPath(string wndName)
        {
            return "Assets/Prefabs/Window/" + wndName;
        }

        public static string GetConfigPath(string name)
        {
            return "Assets/Config/" + name;
        }
    }
}