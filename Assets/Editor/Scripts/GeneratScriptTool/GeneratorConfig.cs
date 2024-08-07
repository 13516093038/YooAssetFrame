using UnityEngine;

namespace YooAssetFrame.Editor
{
    public enum GenerateType
    {
        Window,
        ComponentTool,
    }

    public enum ReferenceType
    {
        //组件查找
        Find,
        //组件绑定
        Bind
    }
    public class GeneratorConfig
    {
        public static string ComponentGeneratorPath = Application.dataPath + "/Scripts/HotUpdate/Window";
        public static string WindowGeneratorPath = Application.dataPath + "/Scripts/HotUpdate/Window";
        public static string OBJDATALIST_KEY = "objDataList";
        public static ReferenceType ReferenceType = ReferenceType.Bind;
    }
}