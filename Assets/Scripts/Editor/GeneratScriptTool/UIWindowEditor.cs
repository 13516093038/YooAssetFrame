using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using File = System.IO.File;

namespace YooAssetFrame.Editor
{
    public class UIWindowEditor : EditorWindow
    {
        private string scriptContent;
        private string filePath;
        private Vector2 scroll = new Vector2();

        /// <summary>
        /// 显示代码展示窗口
        /// </summary>
        /// <param name="content"></param>
        /// <param name="filePath"></param>
        /// <param name="insterDic"></param>
        public static void ShowWindow(string content, string filePath, Dictionary<string, string> insterDic = null)
        {
            //创建代码显示窗口
            UIWindowEditor window = (UIWindowEditor)GetWindowWithRect(typeof(UIWindowEditor),
                new Rect(100, 50, 800, 700), true, "window生成界面");
            window.scriptContent = content;
            window.filePath = filePath;
            //处理代码新增
            if (File.Exists(filePath) & insterDic != null)
            {
                //获取原始代码
                string originScript = File.ReadAllText(filePath);
                foreach (var item in insterDic)
                {
                    //如果代码中没有这个代码进行插入
                    int index = window.GetInsertIndex(originScript);
                    originScript = originScript.Insert(index, item.Value + "\t\t");
                }
            }

            window.Show();
        }

        private void OnGUI()
        {
            //绘制ScrollView
            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(600), GUILayout.Width(800));
            EditorGUILayout.TextArea(scriptContent);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space();

            //绘制脚本生成路径
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextArea("脚本生成路径：" + filePath);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            //绘制按钮
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("生成脚本", GUILayout.Height(30)))
            {
                //按钮事件
                ButtonClick();
            }

            EditorGUILayout.EndHorizontal();
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