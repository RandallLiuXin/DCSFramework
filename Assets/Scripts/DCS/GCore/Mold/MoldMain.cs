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
        // �򿪴��ڵķ���
        [MenuItem("Mold/MoldEditor")]
        static void OpenWindow()
        {
            GetWindow<MoldWindows>("Mold Editor");
        }

        void OnEnable()
        {
            // ���������������ü�������� Inspector��
            // ����Ϊ���ں�Ԥ���е����ý�����һ���Զ��� Inspector��
        }

        void OnDisable()
        {
        }

        void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(Utility.Text.Format("��������mold component")))
            {
                ComponentGenerator.Generator();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(Utility.Text.Format("��������system������")))
            {
                SystemGenerator.Generator();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(Utility.Text.Format("��������command������")))
            {
                CommandGenerator.Generator();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(Utility.Text.Format("��������entity������")))
            {
                EntityGenerator.Generator();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(Utility.Text.Format("��������data tableӳ��")))
            {
                DataTableGenerator.Generator();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(Utility.Text.Format("ȫ����������")))
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
