using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace YooAssetFrame.Editor
{
    [CreateAssetMenu(fileName = "WindowConfig", menuName = "WindowConfig", order = 0)]
    public class WindowConfig :ScriptableObject
    {
        //private string[] windowRootArr = new string[] { "Model", "Test" };
        private string windowRootPath = "Assets/Prefabs/Window";
        public List<WindowData> WindowConfigList = new List<WindowData>();

        public void GeneratorWindowConfig()
        {
            WindowConfigList.Clear();
            string[] filePathArr = Directory.GetFiles(windowRootPath, "*.prefab", SearchOption.AllDirectories);
            foreach (var path in filePathArr)
            {
                if (path.EndsWith(".meta"))
                {
                    continue;
                }
                else
                {
                    //获取预支体名字
                    string fileName = Path.GetFileNameWithoutExtension(path);
                    //计算文件读取路径
                    string filePath = path;
                    WindowData data = new WindowData() { name = fileName, path = filePath };
                    WindowConfigList.Add(data);
                }
            }
        }

        public string GetWindowPath(string winName)
        {
            foreach (var item in WindowConfigList)
            {
                if (item.name == winName)
                {
                    return item.path;
                }
            }

            Debug.LogError("无法在WindowConfigList中找到该window，window name:" + winName);
            return null;
        }
    }

    [Serializable]
    public class WindowData
    {
        public string name;
        public string path;
    }
}