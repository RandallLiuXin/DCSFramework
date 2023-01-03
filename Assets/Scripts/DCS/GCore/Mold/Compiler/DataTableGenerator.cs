using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Galaxy.Command;
using Galaxy.Entities;
using Galaxy.Data;
using Galaxy.DataTable;

namespace Galaxy.Mold
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GalaxyDataTableAttribute : Attribute
    {
        public GalaxyDataTableAttribute(string fileName)
        {
            m_fileName = fileName;
        }

        private string m_fileName;
        public string FileName => m_fileName;
    }
#pragma warning disable CS0168
    public class DataTableGenerator : GeneratorBase
    {
        private const string HeaderImport = @"// auto generate file (mold)
using UnityEngine;
using UnityEditor;
using System;
";

        public static void Generator()
        {
            string[] NamespaceList = new string[] { "Galaxy.DataTable" };
            Dictionary<string, List<Type>> customDataTables = GetAllCustomDataTable(NamespaceList);

            //CSVImportHelper
            {
                string namespaceStr = "Galaxy.DataTable";
                string outputName = UnityConst.ApplicationAssetsPath + @"Editor\DataTable\CSVImportHelper.cs";
                try
                {
                    using (StreamWriter writer = new StreamWriter(outputName))
                    {
                        // header
                        WriteToFile(writer, HeaderImport);

                        WriteToFile(writer, Utility.Text.Format("namespace {0}", namespaceStr));
                        WriteNamespaceStart(writer);
                        // context
                        {
                            using TabCounter counter = new TabCounter();
                            GeneratorDataTableManagerProxy(writer, customDataTables);
                        }

                        //end
                        WriteNamespaceEnd(writer);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            //DataTable helper
            {
                string namespaceStr = "Galaxy.DataTable";
                string outputName = UnityConst.ApplicationAssetsPath + @"Scripts\DCS\GExtend\DataTable\DataTableHelper.cs";
                try
                {
                    using (StreamWriter writer = new StreamWriter(outputName))
                    {
                        // header
                        WriteToFile(writer, HeaderImport);

                        WriteToFile(writer, Utility.Text.Format("namespace {0}", namespaceStr));
                        WriteNamespaceStart(writer);
                        // context
                        {
                            using TabCounter counter = new TabCounter();
                            GeneratorDataTableHelper(writer, customDataTables);
                        }

                        //end
                        WriteNamespaceEnd(writer);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            Debug.Log("生成完成");
        }

        private static Dictionary<string, List<Type>> GetAllCustomDataTable(string[] namespaceList)
        {
            Dictionary<string, List<Type>> customDataTables = new Dictionary<string, List<Type>>();
            foreach (var namespaceStr in namespaceList)
            {
                var result = from customType in Assembly.GetExecutingAssembly().GetTypes() where customType.IsClass && namespaceStr == customType.Namespace select customType;
                foreach (var customType in result.ToList())
                {
                    Attribute[] classAttrs = Attribute.GetCustomAttributes(customType, typeof(GalaxyDataTableAttribute));
                    if (classAttrs.Length == 0)
                        continue;

                    if (!customDataTables.ContainsKey(namespaceStr))
                        customDataTables.Add(namespaceStr, new List<Type>());
                    customDataTables[namespaceStr].Add(customType);
                }
            }
            return customDataTables;
        }

        private static void GeneratorDataTableManagerProxy(StreamWriter writer, Dictionary<string, List<Type>> customDataTables)
        {
            // helper
            WriteToFile(writer, Utility.Text.Format("public partial class CSVImportHelper"));
            WriteNamespaceStart(writer);
            {
                using TabCounter counter1 = new TabCounter();
                WriteToFile(writer, Utility.Text.Format("public static void Deserialize(TextAsset data, string fileName)"));
                WriteNamespaceStart(writer);
                {
                    using TabCounter counter2 = new TabCounter();
                    foreach (var item in customDataTables)
                    {
                        foreach (var dataTableType in item.Value)
                        {
                            try
                            {
                                Attribute[] classAttrs = Attribute.GetCustomAttributes(dataTableType, typeof(GalaxyDataTableAttribute));
                                Debug.Assert(classAttrs.Length == 1);
                                GalaxyDataTableAttribute dataTableAttribute = classAttrs[0] as GalaxyDataTableAttribute;
                                Debug.Assert(dataTableAttribute != null);
                                WriteToFile(writer, Utility.Text.Format("if (fileName == \"{0}\")", dataTableAttribute.FileName));
                                WriteNamespaceStart(writer);
                                {
                                    using TabCounter counter3 = new TabCounter();
                                    WriteToFile(writer, Utility.Text.Format("string outputPath = CSVImportConst.ScriptableObjectOutputPath + fileName;"));
                                    WriteToFile(writer, Utility.Text.Format("string assetfile = outputPath.Replace(\".csv\", \".asset\");"));
                                    WriteNewLine(writer);
                                    WriteToFile(writer, Utility.Text.Format("{0} scriptObj = AssetDatabase.LoadAssetAtPath<{0}>(assetfile);", dataTableType.FullName));
                                    WriteToFile(writer, Utility.Text.Format("if (scriptObj == null)"));
                                    WriteNamespaceStart(writer);
                                    {
                                        using TabCounter counter4 = new TabCounter();
                                        WriteToFile(writer, Utility.Text.Format("scriptObj = ScriptableObject.CreateInstance<{0}>();", dataTableType.FullName));
                                        WriteToFile(writer, Utility.Text.Format("AssetDatabase.CreateAsset(scriptObj, assetfile);"));
                                    }
                                    WriteNamespaceEnd(writer);
                                    WriteNewLine(writer);

                                    WriteToFile(writer, Utility.Text.Format("scriptObj.m_Sample = CSVSerializer.Deserialize<{0}.Sample>(data.text);", dataTableType.FullName));
                                    WriteToFile(writer, Utility.Text.Format("EditorUtility.SetDirty(scriptObj);"));
                                    WriteToFile(writer, Utility.Text.Format("AssetDatabase.SaveAssets();"));
                                }
                                WriteNamespaceEnd(writer);
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                        }
                    }
                }
                WriteNamespaceEnd(writer);
            }
            WriteNamespaceEnd(writer);
        }

        private static void GeneratorDataTableHelper(StreamWriter writer, Dictionary<string, List<Type>> customDataTables)
        {
            Dictionary<string, string> dataTableResPathStrDict = new Dictionary<string, string>();

            // helper
            WriteToFile(writer, Utility.Text.Format("public enum DataTableEnum"));
            WriteNamespaceStart(writer);
            {
                using TabCounter counter = new TabCounter();
                foreach (var item in customDataTables)
                {
                    foreach (var dataTableType in item.Value)
                    {
                        try
                        {
                            Attribute[] classAttrs = Attribute.GetCustomAttributes(dataTableType, typeof(GalaxyDataTableAttribute));
                            Debug.Assert(classAttrs.Length == 1);
                            GalaxyDataTableAttribute dataTableAttribute = classAttrs[0] as GalaxyDataTableAttribute;
                            Debug.Assert(dataTableAttribute != null);
                            string enumName = dataTableAttribute.FileName.Replace(".csv", "");
                            WriteToFile(writer, Utility.Text.Format("{0},", enumName));

                            dataTableResPathStrDict.Add(enumName, enumName);
                            //dataTableResPathStrDict.Add(enumName, dataTableAttribute.FileName);
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }
            }
            WriteNamespaceEnd(writer);
            WriteNewLine(writer);

            WriteToFile(writer, Utility.Text.Format("public class DataTableHelper"));
            WriteNamespaceStart(writer);
            {
                using TabCounter counter1 = new TabCounter();
                WriteToFile(writer, Utility.Text.Format("private static System.Collections.Generic.Dictionary<DataTableEnum, string> ms_DataTableResPathDict = new System.Collections.Generic.Dictionary<DataTableEnum, string>"));
                WriteToFile(writer, "{");
                {
                    using TabCounter counter2 = new TabCounter();
                    foreach (var item in dataTableResPathStrDict)
                    {
                        string tempStr = Utility.Text.Format("DataTableEnum.{0}, \"DataTable/{1}\"", item.Key, item.Value);
                        WriteToFile(writer, "{ " + tempStr + " },");
                    }
                }
                WriteToFile(writer, "};");
                WriteNewLine(writer);

                WriteToFile(writer, Utility.Text.Format("private static System.Collections.Generic.Dictionary<DataTableEnum, ScriptableObject> ms_DataTableScriptableObject = new System.Collections.Generic.Dictionary<DataTableEnum, ScriptableObject>();"));
                WriteNewLine(writer);

                WriteToFile(writer, Utility.Text.Format("public static DataTableEnum GetDataTableEnumByType(Type type)"));
                WriteNamespaceStart(writer);
                {
                    using TabCounter counter2 = new TabCounter();
                    foreach (var item in customDataTables)
                    {
                        foreach (var dataTableType in item.Value)
                        {
                            try
                            {
                                Attribute[] classAttrs = Attribute.GetCustomAttributes(dataTableType, typeof(GalaxyDataTableAttribute));
                                Debug.Assert(classAttrs.Length == 1);
                                GalaxyDataTableAttribute dataTableAttribute = classAttrs[0] as GalaxyDataTableAttribute;
                                Debug.Assert(dataTableAttribute != null);

                                string enumName = dataTableAttribute.FileName.Replace(".csv", "");
                                WriteToFile(writer, Utility.Text.Format("if (type == typeof({0}))", dataTableType.FullName));
                                WriteToFile(writer, Utility.Text.Format("\treturn DataTableEnum.{0};", enumName));
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                        }
                    }
                    WriteToFile(writer, Utility.Text.Format("throw new NotImplementedException();"));
                }
                WriteNamespaceEnd(writer);
                WriteNewLine(writer);

                WriteToFile(writer, Utility.Text.Format("public static ScriptableObject GetDataTable(DataTableEnum dataTableEnum)"));
                WriteNamespaceStart(writer);
                {
                    using TabCounter counter2 = new TabCounter();
                    WriteToFile(writer, Utility.Text.Format("if (ms_DataTableScriptableObject.ContainsKey(dataTableEnum))"));
                    WriteToFile(writer, Utility.Text.Format("\treturn ms_DataTableScriptableObject[dataTableEnum];"));
                    WriteNewLine(writer);
                    WriteToFile(writer, Utility.Text.Format("Debug.Assert(ms_DataTableResPathDict.ContainsKey(dataTableEnum));"));
                    WriteToFile(writer, Utility.Text.Format("string resPath = ms_DataTableResPathDict[dataTableEnum];"));
                    WriteToFile(writer, Utility.Text.Format("ScriptableObject scriptableObject = Resources.Load<ScriptableObject>(resPath);"));
                    WriteToFile(writer, Utility.Text.Format("ms_DataTableScriptableObject.Add(dataTableEnum, scriptableObject);"));
                    WriteToFile(writer, Utility.Text.Format("return scriptableObject;"));
                }
                WriteNamespaceEnd(writer);
                WriteNewLine(writer);

                WriteToFile(writer, Utility.Text.Format("public static T GetDataTable<T>() where T : ScriptableObject"));
                WriteNamespaceStart(writer);
                {
                    using TabCounter counter2 = new TabCounter();
                    WriteToFile(writer, Utility.Text.Format("DataTableEnum dataTableEnum = GetDataTableEnumByType(typeof(T));"));
                    WriteToFile(writer, Utility.Text.Format("ScriptableObject scriptableObject = GetDataTable(dataTableEnum);"));
                    WriteToFile(writer, Utility.Text.Format("T result = scriptableObject as T;"));
                    WriteToFile(writer, Utility.Text.Format("Debug.Assert(result != null);"));
                    WriteToFile(writer, Utility.Text.Format("return result;"));
                }
                WriteNamespaceEnd(writer);
            }
            WriteNamespaceEnd(writer);
        }
    }
#pragma warning restore CS0168
}
