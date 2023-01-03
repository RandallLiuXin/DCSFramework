// auto generate file (mold)
using UnityEngine;
using UnityEditor;
using System;

namespace Galaxy.DataTable
{
	public partial class CSVImportHelper
	{
		public static void Deserialize(TextAsset data, string fileName)
		{
			if (fileName == "sample.csv")
			{
				string outputPath = CSVImportConst.ScriptableObjectOutputPath + fileName;
				string assetfile = outputPath.Replace(".csv", ".asset");
				
				Galaxy.DataTable.CSVImportExample scriptObj = AssetDatabase.LoadAssetAtPath<Galaxy.DataTable.CSVImportExample>(assetfile);
				if (scriptObj == null)
				{
					scriptObj = ScriptableObject.CreateInstance<Galaxy.DataTable.CSVImportExample>();
					AssetDatabase.CreateAsset(scriptObj, assetfile);
				}
				
				scriptObj.m_Sample = CSVSerializer.Deserialize<Galaxy.DataTable.CSVImportExample.Sample>(data.text);
				EditorUtility.SetDirty(scriptObj);
				AssetDatabase.SaveAssets();
			}
		}
	}
}
