using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace YooAssetFrame.Editor
{
    public class GeneratorWindowTool : UnityEditor.Editor
    {
        private static Dictionary<string, string> methodDic = new Dictionary<string, string>();
        
        [MenuItem("GameObject/生成Window脚本")]
        static void CreatFindComponentScripts()
        {
            //获取到当前选择的物体
            GameObject obj = Selection.objects.First() as GameObject;
            if (obj == null)
            {
                Debug.LogError("需要选择GameObject");
                return;
            }
            
            //生成CS脚本
            string csContent = CreateWindowCs(obj.name);
            string dirPath = GeneratorConfig.WindowGeneratorPath + "/" + obj.name;
            string csPath = GeneratorConfig.WindowGeneratorPath + "/" + obj.name + "/"+ obj.name + ".cs";
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            //生成脚本文件
            if (File.Exists(csPath))
            {
                File.Delete(csPath);
            }
            StreamWriter writer = File.CreateText(csPath);
            writer.Write(csContent);
            writer.Close();
            AssetDatabase.Refresh();
            Debug.Log(csPath);
        }

        private static string CreateWindowCs(string name)
        {
            //获取字段名称
            string dataListJson = PlayerPrefs.GetString(GeneratorConfig.OBJDATALIST_KEY);
            List<EditorObjectData> objectDataList = JsonConvert.DeserializeObject<List<EditorObjectData>>(dataListJson);
            string nameSpaceName = "HotUpdate";
            methodDic.Clear();
            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine("/*---------------------------");
            sb.AppendLine(" *Title:UI表现层脚本自动化生成工具");
            sb.AppendLine(" *Author:Jet");
            sb.AppendLine(" *Date:" + System.DateTime.Now);
            sb.AppendLine(" *Description:表现层，该层只负责界面的交互，表现相关的更新，不允许编写任何业务逻辑代码");
            sb.AppendLine(" *注意：以下文件是自动生成的，再次生成不会覆盖原有的代码，会在原油代码上进行新增，可放心使用");
            sb.AppendLine("---------------------------*/");
            sb.AppendLine();
            sb.AppendLine("using UnityEngine.UI;");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine();
            
            //生成命名空间
            if (!string.IsNullOrEmpty(nameSpaceName))
            {
                sb.AppendLine($"namespace {nameSpaceName}");
                sb.AppendLine("{");
            }
            
            //生成类名
            sb.AppendLine($"\tpublic class {name} : WindowBase");
            sb.AppendLine("\t{");
            
            //生成字段
            sb.AppendLine($"\t\tprivate {name}UIComponent uiComp = new {name}UIComponent();");
            //生成生命周期函数
            sb.AppendLine();
            sb.AppendLine("\t\t#region 生命周期函数");
            //OnAwake
            sb.AppendLine("\t\tpublic override void OnAwake()");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tbase.OnAwake();");
            sb.AppendLine("\t\t\tuiComp.InitComponent(this);");
            sb.AppendLine("\t\t}");
            sb.AppendLine();
            //OnShow
            sb.AppendLine("\t\tpublic override void OnShow()");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tbase.OnShow();");
            sb.AppendLine("\t\t}");
            sb.AppendLine();
            //OnHide
            sb.AppendLine("\t\tpublic override void OnHide()");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tbase.OnHide();");
            sb.AppendLine("\t\t}");
            sb.AppendLine();
            
            //OnUpdate
            sb.AppendLine("\t\tpublic override void OnUpdate()");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tbase.OnUpdate();");
            sb.AppendLine("\t\t}");
            sb.AppendLine();
            
            //OnDestroy
            sb.AppendLine("\t\tpublic override void OnDestroy()");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tbase.OnDestroy();");
            sb.AppendLine("\t\t}");
            
            sb.AppendLine("\t\t#endregion");
            
            //APIFunction
            sb.AppendLine("\t\t#region APIFunction");
            sb.AppendLine();
            sb.AppendLine("\t\t#endregion");
            
            //UI组件事件生成
            sb.AppendLine("\t\t#region UI组件事件");
            foreach (var item in objectDataList)
            {
                string type = item.fieldType;
                string methodName = "On" + item.fieldName;
                string suffix = "";

                if (type.Contains("Button"))
                {
                    suffix = "ButtonClick";
                    CreateMethod(sb, methodDic, methodName + suffix);
                }
                else if (type.Contains("InputField"))
                {
                    suffix = "InputChange";
                    CreateMethod(sb, methodDic, methodName + suffix, "string text");
                    suffix = "InputEnd";
                    CreateMethod(sb, methodDic, methodName + suffix, "string text");
                }
                else if(type.Contains("Toggle"))
                {
                    suffix = "ToggleChange";
                    CreateMethod(sb, methodDic, methodName + suffix, "bool state, Toggle toggle");
                }
            }
            sb.AppendLine("\t\t#endregion");

            sb.AppendLine("\t}");
            
            //生成命名空间
            if (!string.IsNullOrEmpty(nameSpaceName))
            {
                sb.AppendLine("}");
            }
            return sb.ToString();
        }

        private static void CreateMethod(StringBuilder sb, Dictionary<string, string> methodDic, string methodName,
            string param = "")
        {
            sb.AppendLine($"\t\tpublic void {methodName}({param})");
            sb.AppendLine("\t\t{");
            if (methodName == "OnCloseButtonClick")
            {
                sb.AppendLine("\t\t\tHideWindow()");
            }
            sb.AppendLine("\t\t}");
            
            //储存UI组件事件，提供给后续新增代码使用
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"\t\tpublic void {methodName}({param})");
            builder.AppendLine("\t\t{");
            builder.AppendLine();
            builder.AppendLine("\t\t}");
            methodDic.Add(methodName, builder.ToString());
        }
    }
}