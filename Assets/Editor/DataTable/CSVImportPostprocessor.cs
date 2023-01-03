using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
namespace Galaxy.DataTable
{
    public class CSVImportPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string str in importedAssets)
            {
                //如果不是csv直接返回
                if (!str.EndsWith(".csv"))
                    continue;
                CSVImportHelper.CSVFileToScriptObj(str);
            }
        }
    }

    public partial class CSVImportHelper
    {
        public static void CSVFileToScriptObj(string path)
        {
            var strs = path.Split('/');
            string fileName = strs[strs.Length - 1];
            if (!fileName.EndsWith(".csv"))
                return;

            TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            if (data == null)
                return;
            Deserialize(data, fileName);
        }

        public static void GenerateScriptObj()
        {
            string CSVStorePath = UnityConst.ApplicationPath + CSVImportConst.CSVStorePath;
            if (Directory.Exists(CSVStorePath))
            {
                DirectoryInfo direction = new DirectoryInfo(CSVStorePath);
                FileInfo[] files = direction.GetFiles("*csv", SearchOption.AllDirectories);

                Debug.Log("we have " + files.Length + " in the csv store path.");
                foreach (var file in files)
                {
                    Debug.Assert(file.Name.EndsWith(".csv"));
                    string path = file.FullName.Replace("\\", "/");
                    Debug.Log("refresh text name: " + path);
                    CSVFileToScriptObj(path.Replace(UnityConst.ApplicationPath, ""));
                }
            }
        }

        public static void DownloadAllCSV()
        {
            foreach (var item in CSVImportConst.CSVUrlToFileName)
            {
                string url = item.Key;
                string fileName = item.Value;
                StartCorountine(DownloadAndImport(url, fileName));
            }
        }

        public static uint RemainCSVDownloadNumber = 0;

        public static IEnumerator DownloadAndImport(string url, string csvFileName)
        {
            UnityWebRequest www = UnityWebRequest.Get(url);
            RemainCSVDownloadNumber++;
            yield return www.SendWebRequest();

            while (www.isDone == false)
            {
                yield return new WaitForEndOfFrame();
            }

            if (www.error != null)
            {
                Debug.Log("UnityWebRequest.error:" + www.error);
            }
            else if (www.downloadHandler.text == "" || www.downloadHandler.text.IndexOf("<!DOCTYPE") != -1)
            {
                Debug.Log("Uknown Format:" + www.downloadHandler.text);
            }
            else
            {
                string path = CSVImportConst.CSVStorePath + csvFileName;
                TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                if (asset == null)
                {
                    asset = new TextAsset(www.downloadHandler.text);
                    AssetDatabase.CreateAsset(asset, path);
                }
                else
                {
                    using (StreamWriter writer = new StreamWriter(UnityConst.ApplicationPath + path))
                    {
                        writer.WriteLine(www.downloadHandler.text);
                        writer.Close();
                        writer.Dispose();
                    }
                }
                EditorUtility.SetDirty(asset);
                AssetDatabase.SaveAssets();

                CSVFileToScriptObj(path);
                RemainCSVDownloadNumber--;
                Debug.Log("Download Asset: " + path + " Remain download task: " + RemainCSVDownloadNumber);
            }
        }

        // coroutine for unity editor
        public static void StartCorountine(IEnumerator routine)
        {
            _coroutine.Add(routine);
            if (_coroutine.Count == 1)
                EditorApplication.update += ExecuteCoroutine;
        }
        public static List<IEnumerator> _coroutine = new List<IEnumerator>();
        public static void ExecuteCoroutine()
        {
            for (int i = 0; i < _coroutine.Count;)
            {
                if (_coroutine[i] == null || !_coroutine[i].MoveNext())
                    _coroutine.RemoveAt(i);
                else
                    i++;
            }
            if (_coroutine.Count == 0)
                EditorApplication.update -= ExecuteCoroutine;
        }
    }
}
#endif
