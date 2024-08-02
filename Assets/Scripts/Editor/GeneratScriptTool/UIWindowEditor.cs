using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using File = System.IO.File;

namespace YooAssetFrame.Editor
{
    public enum GenerateType
    {
        Window,
        ComponentTool,
    }
    
    public class UIWindowEditor : EditorWindow
    {
        private string scriptContent;
        private string windowContent;
        private string filePath;
        private Vector2 scroll = new Vector2();

        /// <summary>
        /// 显示代码展示窗口
        /// </summary>
        /// <param name="content"></param>
        /// <param name="filePath"></param>
        /// <param name="insterDic"></param>
        public static void ShowWindow(GenerateType type, string content, string filePath, Dictionary<string, string> insterDic = null)
        {
            //创建代码显示窗口
            UIWindowEditor window = (UIWindowEditor)GetWindowWithRect(typeof(UIWindowEditor),
                new Rect(100, 50, 800, 700), true, "window生成界面");
            window.scriptContent = content;
            window.windowContent = content;
            window.filePath = filePath;
            //处理代码新增
            if (File.Exists(filePath) && type == GenerateType.Window)
            {
                string originScript = File.ReadAllText(filePath);
                window.windowContent = File.ReadAllText(filePath);
                //获取原始代码
                if (insterDic != null)
                {
                    int dicIndex = 0;
                    foreach (var item in insterDic)
                    {
                        if (originScript.Contains(item.Key))
                        {
                            continue;
                        }

                        //如果代码中没有这个代码进行插入
                        int index = window.GetInsertIndex(originScript);
                        
                        if (dicIndex == 0)
                        {
                            window.windowContent = window.windowContent.Insert(index, "<color=red></color>");
                        }
                        window.windowContent = window.windowContent.Insert(index + "<color=red>".Length, item.Value + "\t\t");
                        originScript = originScript.Insert(index, item.Value + "\t\t");
                        dicIndex++;
                    }
                }

                window.scriptContent = originScript;
            }

            window.Show();
        }

        private void OnGUI()
        {
            
            //绘制ScrollView
            scroll = GUILayout.BeginScrollView(scroll, GUILayout.Height(600), GUILayout.Width(800));
            //GUILayout.Label(windowContent, new GUIStyle(GUI.skin.label)
            GUILayout.Label(windowContent, new GUIStyle(GUI.skin.label)
            {
                richText = true
            });
            GUILayout.EndScrollView();
            GUILayout.Space(50);

            //绘制脚本生成路径
            GUILayout.BeginHorizontal();
            GUILayout.TextArea("脚本生成路径：" + filePath);
            GUILayout.EndHorizontal();
            //GUILayout.Space(50);

            //绘制按钮
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("生成脚本", GUILayout.Height(30)))
            {
                //按钮事件
                ButtonClick();
            }

            GUILayout.EndHorizontal();
        }
        
        void ButtonClick()
        {
            //生成脚本文件
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            StreamWriter writer = File.CreateText(filePath);
            writer.Write(scriptContent);
            writer.Close();
            AssetDatabase.Refresh();
            if (EditorUtility.DisplayDialog("自动化生成工具", "生成脚本成功！", "确定"))
            {
                Close();
            }
        }

        /// <summary>
        /// 获取插入代码的下标
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public int GetInsertIndex(string content)
        {
            Regex regex = new Regex("UI组件事件");
            Match match = regex.Match(content);

            Regex regex1 = new Regex("public");
            MatchCollection matchCollection = regex1.Matches(content);

            for (int i = 0; i < matchCollection.Count; i++)
            {
                if (matchCollection[i].Index > match.Index)
                {
                    return matchCollection[i].Index;
                }
            }

            return -1;
        }
    }
}