using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Galaxy.DataTable
{
    public class CSVImportEditor : EditorWindow
    {
        [MenuItem("Tools/CSVImport")]
        static void OpenWindow()
        {
            GetWindow<CSVImportEditor>("CSVImport");
        }

        void OnEnable()
        {
        }

        void OnDisable()
        {
        }

        void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(Utility.Text.Format("刷新游戏内的所有数据")))
            {
                CSVImportHelper.GenerateScriptObj();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(Utility.Text.Format("下载所有Google excel数据并更新")))
            {
                Debug.LogError("暂不支持,没有合适的方式保证隐私同时保证方便");
                //CSVImportHelper.DownloadAllCSV();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
