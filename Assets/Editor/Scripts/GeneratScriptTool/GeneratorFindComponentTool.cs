using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace YooAssetFrame.Editor
{
    public class GeneratorFindComponentTool : UnityEditor.Editor
    {
        //key: 物体的insid, value:物体的查找路径
        public static Dictionary<int, string> objFindPathDic;
        public static List<EditorObjectData> objDataList;

        [MenuItem("GameObject/生成组件查找脚本(shift + U) #U")]
        static void CreateFindComponentScripts()
        {
            //获取到当前选择的物体
            GameObject obj = Selection.objects.First() as GameObject;
            if (obj == null)
            {
                Debug.LogError("需要选择GameObject");
                return;
            }

            objFindPathDic = new Dictionary<int, string>();
            objDataList = new List<EditorObjectData>();

            //解析窗口组件数据
            PresWindowNodeData(obj.transform, obj.name);
            
            //储存字段名称
            string dataListJson = JsonConvert.SerializeObject(objDataList);
            PlayerPrefs.SetString(GeneratorConfig.OBJDATALIST_KEY, dataListJson);
            
            //生成CS脚本
            string csContent = CreateCS(obj.name);
            string dirPath = GeneratorConfig.ComponentGeneratorPath + "/" + obj.name;
            string csPath = GeneratorConfig.ComponentGeneratorPath + "/" + obj.name + "/"+ obj.name + "UIComponent.cs";
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            UIWindowEditor.ShowWindow( GenerateType.ComponentTool, csContent, csPath);
        }

        /// <summary>
        /// 解析窗口节点数据
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="winName"></param>
        private static void PresWindowNodeData(Transform trans, string winName)
        {
            for (int i = 0; i < trans.childCount; i++)
            {
                GameObject obj = trans.GetChild(i).gameObject;
                string name = obj.name;
                if (name.Contains("[") && name.Contains("]"))
                {
                    int index = name.IndexOf("]") + 1;
                    //获取字段昵称
                    string fieldName = name.Substring(index, name.Length - index);
                    //获取字段类型
                    string fieldType = name.Substring(1, index - 2);
                    
                    objDataList.Add(new EditorObjectData{fieldName = fieldName, fieldType = fieldType, insID = obj.GetInstanceID()});
                    
                    //计算该节点查找路径
                    string objPath = name;
                    Transform parent = obj.transform;

                    while (parent.name != winName)
                    {
                        parent = parent.parent;
                        if (string.Equals(parent.name, winName))
                        {
                            break;
                        }
                        else
                        {
                            objPath = objPath.Insert(0, parent.name + "/");
                        }
                    }
                    objFindPathDic.Add(obj.GetInstanceID(),objPath);
                }
                PresWindowNodeData(trans.GetChild(i), winName);
            }
        }

        private static string CreateCS(string name)
        {
            StringBuilder sb = new StringBuilder();
            string nameSpaceName = "HotUpdate";

            sb.AppendLine("/*---------------------------");
            sb.AppendLine(" *Title:UI自动化组件查找代码工具");
            sb.AppendLine(" *Author:Jet");
            sb.AppendLine(" *Date:" + System.DateTime.Now);
            sb.AppendLine(" *Description:变量需以[Text]括号加组件类型的格式进行声明，然后右键窗口物体，一键生成UI组件查找脚本即可");
            sb.AppendLine(" *注意：以下文件是自动生成的，任何手动修改都会被下次生成覆盖，若手动修改后，尽量避免自动生成");
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

            sb.AppendLine($"\tpublic class {name + "UIComponent"}");
            sb.AppendLine("\t{");
            
            //根据字段数据列表声明字段
            foreach (var item in objDataList)
            {
                sb.AppendLine("\t\tpublic " + item.fieldType + " " + item.fieldName + item.fieldType + ";\n");
            }
            
            //声明接口初始化组件
            sb.AppendLine("\t\tpublic void InitComponent(WindowBase target)");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\t//组件查找");
            
            //根据查找字典路径和字段数据列表生成组件查找代码
            foreach (var item in objFindPathDic)
            {
                EditorObjectData itemdata = GetEditorObjectData(item.Key);
                string relFieldname = itemdata.fieldName + itemdata.fieldType;
                if (itemdata.fieldType == "GameObject")
                {
                    sb.AppendLine($"\t\t\t{relFieldname} = target.transform.Find(\"{item.Value}\").gameObject;");
                }
                else if (itemdata.fieldType == "Transform")
                {
                    sb.AppendLine($"\t\t\t{relFieldname} = target.transform.Find(\"{item.Value}\").Transform;");
                }
                else
                {
                    sb.AppendLine($"\t\t\t{relFieldname} = target.transform.Find(\"{item.Value}\").GetComponent<{itemdata.fieldType}>();");
                }
            }

            sb.AppendLine("\t");
            sb.AppendLine("\t\t\t//组件事件绑定");
            //得到逻辑类 WindowBase -> LoginWindow
            sb.AppendLine($"\t\t\t{name} mWindow = ({name})target;");
            
            //生成UI事件绑定代码
            foreach (var item in objDataList)
            {
                string type = item.fieldType;
                string methodName = item.fieldName;
                string suffix = "";

                if (type.Contains("Button"))
                {
                    suffix = "Click";
                    sb.AppendLine($"\t\t\ttarget.AddButtonClickListener({methodName}{type},mWindow.On{methodName}Button{suffix});");
                }
                
                if (type.Contains("InputField"))
                {
                    sb.AppendLine($"\t\t\ttarget.AddInputFieldListener({methodName}{type},mWindow.On{methodName}InputChange, mWindow.On{methodName}InputEnd);");
                }
                
                if (type.Contains("Toggle"))
                {
                    suffix = "Change";
                    sb.AppendLine($"\t\t\ttarget.AddToggleClickListener({methodName}{type},mWindow.On{methodName}Button{suffix});");
                }
            }

            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");
            if (!string.IsNullOrEmpty(nameSpaceName))
            {
                sb.AppendLine("}");
            }

            return sb.ToString();
        }

        private static EditorObjectData GetEditorObjectData(int insid)
        {
            foreach (var item in objDataList)
            {
                if (item.insID == insid)
                {
                    return item;
                }
            }

            return null;
        }
    }
    
    public class EditorObjectData
    {
        public int insID;
        public string fieldName;
        public string fieldType;
    }
}

