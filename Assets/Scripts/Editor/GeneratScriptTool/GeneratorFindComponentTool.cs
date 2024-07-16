using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace YooAssetFrame.Editor
{
    public class GeneratorFindComponentTool : UnityEditor.Editor
    {
        //key: 物体的insid, value:物体的查找路径
        public static Dictionary<int, string> objFindPathDic;
        public static List<EditorObjectData> objDataList;

        [MenuItem("GameObject/生成组件查找脚本")]
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
            
            //设置脚本生成路径
            if (!Directory.Exists(GeneratorConfig.FindComponentGeneratorPath))
            {
                Directory.CreateDirectory(GeneratorConfig.FindComponentGeneratorPath);
            }

            PresWindowNodeData(obj.transform, obj.name);
        }

        /// <summary>
        /// 解析窗口节点数据
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="winName"></param>
        public static void PresWindowNodeData(Transform trans, string winName)
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
    }
    
    public class EditorObjectData
    {
        public int insID;
        public string fieldName;
        public string fieldType;
    }
}

