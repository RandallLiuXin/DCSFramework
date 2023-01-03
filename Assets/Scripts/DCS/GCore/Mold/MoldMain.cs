using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Galaxy.Mold
{
    public class MoldMain
    {
        public static readonly bool IsRelease = true;

    }

    public class MoldWindows : EditorWindow
    {
        // 打开窗口的方法
        [MenuItem("Mold/MoldEditor")]
        static void OpenWindow()
        {
            GetWindow<MoldWindows>("Mold Editor");
        }

        void OnEnable()
        {
            // 立即创建您的设置及其关联的 Inspector，
            // 用于为窗口和预设中的设置仅创建一个自定义 Inspector。
        }

        void OnDisable()
        {
        }

        void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(Utility.Text.Format("生成所有mold component")))
            {
                ComponentGenerator.Generator();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(Utility.Text.Format("生成所有system代理类")))
            {
                SystemGenerator.Generator();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(Utility.Text.Format("生成所有command代理类")))
            {
                CommandGenerator.Generator();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(Utility.Text.Format("生成所有entity代理类")))
            {
                EntityGenerator.Generator();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(Utility.Text.Format("生成所有data table映射")))
            {
                DataTableGenerator.Generator();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(Utility.Text.Format("全部重新生成")))
            {
                ComponentGenerator.Generator();
                SystemGenerator.Generator();
                CommandGenerator.Generator();
                EntityGenerator.Generator();
                DataTableGenerator.Generator();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
