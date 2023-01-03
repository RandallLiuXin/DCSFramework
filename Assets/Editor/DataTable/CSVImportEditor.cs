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
            if (GUILayout.Button(Utility.Text.Format("ˢ����Ϸ�ڵ���������")))
            {
                CSVImportHelper.GenerateScriptObj();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(Utility.Text.Format("��������Google excel���ݲ�����")))
            {
                Debug.LogError("�ݲ�֧��,û�к��ʵķ�ʽ��֤��˽ͬʱ��֤����");
                //CSVImportHelper.DownloadAllCSV();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
