using UnityEditor;
using YooAssetFrame.Editor;

namespace Editor.Scripts.Tools
{
    public class EditorTools : EditorWindow
    {
        [MenuItem("Tools/更新UI预制体路径配置文件", priority = 1)]
        
        public static void UpdateWindowConfig()
        {
            WindowConfig windowConfig =
                AssetDatabase.LoadAssetAtPath<WindowConfig>("Assets/Config/WindowConfig.asset");
            windowConfig.GeneratorWindowConfig();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}