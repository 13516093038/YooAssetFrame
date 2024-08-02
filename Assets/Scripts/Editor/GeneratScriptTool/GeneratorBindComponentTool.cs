using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace YooAssetFrame.Editor
{
    public class GeneratorBindComponentTool : UnityEditor.Editor
    {
        public static List<EditorObjectData> objDataList;

        [MenuItem("GameObject/生成组件数据脚本")]
        static void CreateFindComponentScripts()
        {
            //获取到当前选择的物体
            GameObject obj = Selection.objects.First() as GameObject;
            if (obj == null)
            {
                Debug.LogError("需要选择GameObject");
                return;
            }
            objDataList = new List<EditorObjectData>();

            //解析窗口组件数据
            PresWindowNodeData(obj.transform);
            
            //储存字段名称
            string dataListJson = JsonConvert.SerializeObject(objDataList);
            PlayerPrefs.SetString(GeneratorConfig.OBJDATALIST_KEY, dataListJson);
            
            //生成CS脚本
            string csContent = CreateCS(obj.name);
            string dirPath = GeneratorConfig.ComponentGeneratorPath + "/" + obj.name;
            string csPath = dirPath + "/" + obj.name + "DataComponent" + ".cs";
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            UIWindowEditor.ShowWindow( GenerateType.ComponentTool, csContent, csPath);
            EditorPrefs.SetString("GeneratorClassname", obj.name + "DataComponent");
        }

        /// <summary>
        /// 解析窗口节点数据
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="winName"></param>
        private static void PresWindowNodeData(Transform trans)
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
                }
                PresWindowNodeData(trans.GetChild(i));
            }
        }

        private static string CreateCS(string name)
        {
            StringBuilder sb = new StringBuilder();
            string nameSpaceName = "HotUpdate";

            sb.AppendLine("/*---------------------------");
            sb.AppendLine(" *Title:UI自动化组件生成代码工具");
            sb.AppendLine(" *Author:Jet");
            sb.AppendLine(" *Date:" + System.DateTime.Now);
            sb.AppendLine(" *Description:变量需以[Text]括号加组件类型的格式进行声明，然后右键窗口物体，一键生成UI数据组件脚本即可");
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

            sb.AppendLine($"\tpublic class {name + "DataComponent : MonoBehaviour"}");
            sb.AppendLine("\t{");
            
            //根据字段数据列表声明字段
            foreach (var item in objDataList)
            {
                sb.AppendLine("\t\tpublic " + item.fieldType + " " + item.fieldName + item.fieldType + ";\n");
            }
            
            //声明接口初始化组件
            sb.AppendLine("\t\tpublic void InitComponent(WindowBase target)");
            sb.AppendLine("\t\t{");
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

        /// <summary>
        /// 编译完成之后系统自动调用
        /// </summary>
        [UnityEditor.Callbacks.DidReloadScripts]
        public static void AddComponent2Window()
        {
            //如果当前不是处理数据脚本回调，就不处理
            string classname = EditorPrefs.GetString("GeneratorClassname");
            if (string.IsNullOrEmpty(classname))
            {
                return;
            }
            //1.通过反射的方式从程序集中找到这个脚本，把它挂到当前的物体上
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            //找到C#程序集
            var cSharpAssembly = assemblies.First(assembly => assembly.GetName().Name == "HotUpdate");
            //获取类所在的程序集路径
            string relClassName = "HotUpdate." + classname;
            System.Type type = cSharpAssembly.GetType(relClassName);
            if (type == null)
            {
                Debug.LogError("type为空");
                return;
            }
            //获取要挂载的物体
            string windowObjName = classname.Replace("DataComponent", "");
            GameObject windowObj =
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Window/" + windowObjName + ".prefab");
            windowObj = PrefabUtility.InstantiatePrefab(windowObj) as GameObject;
            if (windowObj == null)
            {
                Debug.LogError("物体为空,物体名：" + windowObjName);
                return;
            }
            //获取窗口上有没有挂载数据组件，如果没有进行挂载
            Component comp = windowObj.GetComponent(type);
            if (comp == null)
            {
                comp = windowObj.AddComponent(type);
            }
            //2.通过反射的方式，遍历数据列表，找到对应的字段，赋值
            //获取对象数据列表
            string datalistjson = PlayerPrefs.GetString(GeneratorConfig.OBJDATALIST_KEY);
            List<EditorObjectData> objDataList = JsonConvert.DeserializeObject<List<EditorObjectData>>(datalistjson);

            //获取脚本所有字段
            FieldInfo[] fieldInfoList = type.GetFields();
            
            foreach (var item in fieldInfoList)
            {
                foreach (var objData in objDataList)
                {
                    if (item.Name == objData.fieldName + objData.fieldType)
                    {
                        GameObject uiObject = EditorUtility.InstanceIDToObject(objData.insID) as GameObject;
                        //设置该字段所对应的对象
                        if (string.Equals(objData.fieldName, "GameObject"))
                        {
                            item.SetValue(comp, uiObject);
                        }
                        else
                        {
                            var asd = uiObject.GetComponent(objData.fieldType);
                            item.SetValue(comp, asd);
                        }
                        break;
                    }
                }
            }
            PrefabUtility.ApplyPrefabInstance(windowObj, InteractionMode.UserAction);
            DestroyImmediate(windowObj);
            EditorPrefs.DeleteKey("GeneratorClassname");
        }
    }
}