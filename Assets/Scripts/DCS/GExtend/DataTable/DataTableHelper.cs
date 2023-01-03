// auto generate file (mold)
using UnityEngine;
using UnityEditor;
using System;

namespace Galaxy.DataTable
{
	public enum DataTableEnum
	{
		sample,
	}
	
	public class DataTableHelper
	{
		private static System.Collections.Generic.Dictionary<DataTableEnum, string> ms_DataTableResPathDict = new System.Collections.Generic.Dictionary<DataTableEnum, string>
		{
			{ DataTableEnum.sample, "DataTable/sample" },
		};
		
		private static System.Collections.Generic.Dictionary<DataTableEnum, ScriptableObject> ms_DataTableScriptableObject = new System.Collections.Generic.Dictionary<DataTableEnum, ScriptableObject>();
		
		public static DataTableEnum GetDataTableEnumByType(Type type)
		{
			if (type == typeof(Galaxy.DataTable.CSVImportExample))
				return DataTableEnum.sample;
			throw new NotImplementedException();
		}
		
		public static ScriptableObject GetDataTable(DataTableEnum dataTableEnum)
		{
			if (ms_DataTableScriptableObject.ContainsKey(dataTableEnum))
				return ms_DataTableScriptableObject[dataTableEnum];
			
			Debug.Assert(ms_DataTableResPathDict.ContainsKey(dataTableEnum));
			string resPath = ms_DataTableResPathDict[dataTableEnum];
			ScriptableObject scriptableObject = Resources.Load<ScriptableObject>(resPath);
			ms_DataTableScriptableObject.Add(dataTableEnum, scriptableObject);
			return scriptableObject;
		}
		
		public static T GetDataTable<T>() where T : ScriptableObject
		{
			DataTableEnum dataTableEnum = GetDataTableEnumByType(typeof(T));
			ScriptableObject scriptableObject = GetDataTable(dataTableEnum);
			T result = scriptableObject as T;
			Debug.Assert(result != null);
			return result;
		}
	}
}
