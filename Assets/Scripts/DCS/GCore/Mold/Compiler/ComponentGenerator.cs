using Galaxy.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Galaxy.Mold
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GalaxyComponentAttribute : Attribute
    {
        public GalaxyComponentAttribute(string compName, CompType compType)
        {
            m_CompNameStr = compName;
            m_RealCompType = compType;
            m_CompTypeStr = "Entities.CompType." + compType.ToString();
        }

        private string m_CompNameStr;
        private string m_CompTypeStr;
        private CompType m_RealCompType;

        public string CompName => m_CompNameStr;
        public string CompType => m_CompTypeStr;
        public CompType RealCompType => m_RealCompType;
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class GalaxyPropertyAttribute : Attribute
    {
        public GalaxyPropertyAttribute()
        {
            m_PropertyInitValueStr = "";
        }
        public GalaxyPropertyAttribute(string initValue)
        {
            m_PropertyInitValueStr = initValue;
        }

        private string m_PropertyInitValueStr;
        public string PropertyInitValueStr => m_PropertyInitValueStr;
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class GalaxyCombatPropertyAttribute : GalaxyPropertyAttribute
    {
        public GalaxyCombatPropertyAttribute()
        {
        }
        public GalaxyCombatPropertyAttribute(string maxPropertyName = "", string initValue = "") : base(initValue)
        {
            m_MaxPropertyNameStr = maxPropertyName;
        }

        private string m_MaxPropertyNameStr;
        public string MaxPropertyNameStr => m_MaxPropertyNameStr;
    }
#pragma warning disable CS0168
    public sealed class ComponentGenerator : GeneratorBase
    {
        private const string OutputExtendFileName = ".Mold.cs";
        private const string HeaderImport = @"// auto generate file (mold)
using Galaxy.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
";

        public static void Generator()
        {
            string filesPath = UnityConst.DCSScriptsPath + @"GExtend\Component\ComponentProxy\";
            string[] NamespaceList = GetAllNamespaces();
            Dictionary<string, List<Type>> customComponents = GetAllCustomComponent(NamespaceList);

            //customType.FullName
            foreach (var item in customComponents)
            {
                string namespaceStr = item.Key;
                string outputName = filesPath + namespaceStr + OutputExtendFileName;
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
                            foreach (var component in item.Value)
                            {
                                GeneratorComponent(writer, component);
                            }
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

            //component manager
            {
                string namespaceStr = "Galaxy.Entities";
                string outputName = filesPath + "Galaxy.ComponentHelper.cs";
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
                            GeneratorComponentHelper(writer, customComponents);
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

        private static Dictionary<string, List<Type>> GetAllCustomComponent(string[] namespaceList)
        {
            Dictionary<string, List<Type>> customComponents = new Dictionary<string, List<Type>>();
            foreach (var namespaceStr in namespaceList)
            {
                var result = from customType in Assembly.GetExecutingAssembly().GetTypes() where customType.IsClass && namespaceStr == customType.Namespace select customType;
                foreach (var customType in result.ToList())
                {
                    Attribute[] classAttrs = Attribute.GetCustomAttributes(customType, typeof(GalaxyComponentAttribute));
                    if (classAttrs.Length == 0)
                        continue;

                    if (!customComponents.ContainsKey(namespaceStr))
                        customComponents.Add(namespaceStr, new List<Type>());
                    customComponents[namespaceStr].Add(customType);
                }
            }
            return customComponents;
        }

        private static void GeneratorComponent(StreamWriter writer, Type componentType)
        {
            try
            {
                Attribute[] classAttrs = Attribute.GetCustomAttributes(componentType, typeof(GalaxyComponentAttribute));
                Debug.Assert(classAttrs.Length == 1);
                GalaxyComponentAttribute componentAttribute = classAttrs[0] as GalaxyComponentAttribute;
                Debug.Assert(componentAttribute != null);

                // class
                WriteToFile(writer, Utility.Text.Format("[Serializable]"));
                WriteToFile(writer, Utility.Text.Format("public class {0} : ComponentBase", componentAttribute.CompName));
                WriteNamespaceStart(writer);
                {
                    using TabCounter counter1 = new TabCounter();
                    //constructor
                    WriteToFile(writer, Utility.Text.Format("public {0}()", componentAttribute.CompName));
                    WriteNamespaceStart(writer);
                    WriteNamespaceEnd(writer);

                    //copy constructor
                    WriteToFile(writer, Utility.Text.Format("public {0}({0} component)", componentAttribute.CompName));
                    WriteNamespaceStart(writer);
                    {
                        using TabCounter counter2 = new TabCounter();
                        // property
                        foreach (var propertyInfo in componentType.GetFields())
                        {
                            Attribute[] propertyAttrs = Attribute.GetCustomAttributes(propertyInfo, typeof(GalaxyPropertyAttribute));
                            if (propertyAttrs.Length == 0)
                                continue;

                            Debug.Assert(propertyAttrs.Length == 1);
                            var property = propertyAttrs[0] as GalaxyPropertyAttribute;
                            Debug.Assert(property != null);
                            WriteToFile(writer, Utility.Text.Format("{0} = component.{0};", propertyInfo.Name));
                        }
                    }
                    WriteNamespaceEnd(writer);

                    //CompType define
                    WriteToFile(writer, Utility.Text.Format("public override CompType GetCompType()"));
                    WriteNamespaceStart(writer);
                    {
                        using TabCounter counter2 = new TabCounter();
                        WriteToFile(writer, Utility.Text.Format("return {0};", componentAttribute.CompType));
                    }
                    WriteNamespaceEnd(writer);

                    // property
                    foreach (var propertyInfo in componentType.GetFields())
                    {
                        Attribute[] propertyAttrs = Attribute.GetCustomAttributes(propertyInfo, typeof(GalaxyPropertyAttribute));
                        if (propertyAttrs.Length == 0)
                            continue;

                        Debug.Assert(propertyAttrs.Length == 1);
                        var property = propertyAttrs[0] as GalaxyPropertyAttribute;

                        WriteToFile(writer, Utility.Text.Format("private {0} m_{1} = {2};",
                            GetPropertyTypeString(propertyInfo.FieldType),
                            propertyInfo.Name,
                            property.PropertyInitValueStr.IsNE() ? GetPropertyInitValueStr(propertyInfo.FieldType) : property.PropertyInitValueStr));
                        WriteToFile(writer, Utility.Text.Format("public {0} {1}", GetPropertyReturnTypeString(propertyInfo.FieldType), propertyInfo.Name));
                        WriteNamespaceStart(writer);
                        {
                            using TabCounter counter2 = new TabCounter();
                            WriteToFile(writer, Utility.Text.Format("get"));
                            WriteNamespaceStart(writer);
                            {
                                using TabCounter counter3 = new TabCounter();
                                WriteToFile(writer, Utility.Text.Format("return {0};", GetPropertyValueStr(propertyInfo.FieldType, propertyInfo.Name)));
                            }
                            WriteNamespaceEnd(writer);

                            WriteToFile(writer, Utility.Text.Format("set"));
                            WriteNamespaceStart(writer);
                            {
                                using TabCounter counter3 = new TabCounter();
                                WriteToFile(writer, Utility.Text.Format("{0} = value;", GetPropertyValueStr(propertyInfo.FieldType, propertyInfo.Name)));
                            }
                            WriteNamespaceEnd(writer);
                        }
                        WriteNamespaceEnd(writer);
                    }

                    // Custom generator
                    GeneratorForCustomComponent(writer, componentType);
                }
                WriteNamespaceEnd(writer);

                WriteToFile(writer, Utility.Text.Format("public class {0}Proxy : ComponentProxy", componentAttribute.CompName));
                WriteNamespaceStart(writer);
                {
                    using TabCounter counter1 = new TabCounter();
                    WriteToFile(writer, Utility.Text.Format("private {0} m_Data;", componentAttribute.CompName));
                    WriteToFile(writer, Utility.Text.Format("public {0}Proxy({0} data, AccessType accessType) : base(data, accessType)", componentAttribute.CompName));
                    WriteNamespaceStart(writer);
                    {
                        using TabCounter counter2 = new TabCounter();
                        WriteToFile(writer, "m_Data = data;");
                    }
                    WriteNamespaceEnd(writer);

                    WriteToFile(writer, Utility.Text.Format("public override ComponentBase GetComponentBase()"));
                    WriteNamespaceStart(writer);
                    {
                        using TabCounter counter2 = new TabCounter();
                        WriteToFile(writer, "return m_Data;");
                    }
                    WriteNamespaceEnd(writer);

                    WriteToFile(writer, Utility.Text.Format("public CompType GetCompType()"));
                    WriteNamespaceStart(writer);
                    {
                        using TabCounter counter2 = new TabCounter();
                        WriteToFile(writer, Utility.Text.Format("return m_Data.GetCompType();"));
                    }
                    WriteNamespaceEnd(writer);

                    // property proxy: readonly and readwrite
                    foreach (var propertyInfo in componentType.GetFields())
                    {
                        Attribute[] propertyAttrs = Attribute.GetCustomAttributes(propertyInfo, typeof(GalaxyPropertyAttribute));
                        if (propertyAttrs.Length == 0)
                            continue;

                        Debug.Assert(propertyAttrs.Length == 1);
                        var property = propertyAttrs[0] as GalaxyPropertyAttribute;

                        WriteToFile(writer, Utility.Text.Format("public {0} {1}", GetPropertyReturnTypeString(propertyInfo.FieldType), propertyInfo.Name));
                        WriteNamespaceStart(writer);
                        {
                            using TabCounter counter2 = new TabCounter();
                            WriteToFile(writer, Utility.Text.Format("get"));
                            WriteNamespaceStart(writer);
                            {
                                using TabCounter counter3 = new TabCounter();
                                WriteToFile(writer, Utility.Text.Format("return m_Data.{0};", propertyInfo.Name));
                            }
                            WriteNamespaceEnd(writer);

                            WriteToFile(writer, Utility.Text.Format("set"));
                            WriteNamespaceStart(writer);
                            {
                                using TabCounter counter3 = new TabCounter();
                                WriteToFile(writer, Utility.Text.Format("if (m_AccessType != AccessType.ReadWrite)"));
                                WriteNamespaceStart(writer);
                                {
                                    using TabCounter counter4 = new TabCounter();
                                    WriteToFile(writer, Utility.Text.Format("throw new GalaxyException({0});", "\"Only can modify readwrite component!\""));
                                }
                                WriteNamespaceEnd(writer);
                                WriteToFile(writer, Utility.Text.Format("m_Data.{0} = value;", propertyInfo.Name));
                            }
                            WriteNamespaceEnd(writer);
                        }
                        WriteNamespaceEnd(writer);
                    }

                    // Custom generator
                    GeneratorForCustomComponentProxy(writer, componentType);
                }
                WriteNamespaceEnd(writer);

                // Custom generator
                GeneratorForExtendClass(writer, componentType);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void GeneratorForCustomComponent(StreamWriter writer, Type componentType)
        {
            GeneratorForGalaxyCombat(writer, componentType);
            //...
        }

        private static void GeneratorForCustomComponentProxy(StreamWriter writer, Type componentType)
        {
            GeneratorForGalaxyCombatProxy(writer, componentType);
            //...
        }

        private static void GeneratorForExtendClass(StreamWriter writer, Type componentType)
        {
            GeneratorForGalaxyCombatExtend(writer, componentType);
            //...
        }

        #region GalaxyCombatPropertyAttribute

        private static void GeneratorForGalaxyCombat(StreamWriter writer, Type componentType)
        {
            bool needGenerate = false;
            foreach (var propertyInfo in componentType.GetFields())
            {
                Attribute[] propertyAttrs = Attribute.GetCustomAttributes(propertyInfo, typeof(GalaxyCombatPropertyAttribute));
                if (propertyAttrs.Length == 0)
                    continue;

                Debug.Assert(propertyAttrs.Length == 1);
                var property = propertyAttrs[0] as GalaxyCombatPropertyAttribute;
                if (property == null)
                    Debug.Assert(false);

                needGenerate = true;
                break;
            }

            if (!needGenerate)
                return;

            Attribute[] classAttrs = Attribute.GetCustomAttributes(componentType, typeof(GalaxyComponentAttribute));
            Debug.Assert(classAttrs.Length == 1);
            GalaxyComponentAttribute componentAttribute = classAttrs[0] as GalaxyComponentAttribute;
            Debug.Assert(componentAttribute != null);

            //SetProperty int value generator
            WriteToFile(writer, Utility.Text.Format("public void SetProperty({0}Enum propertyType, int value)", componentAttribute.CompName));
            WriteNamespaceStart(writer);
            {
                using TabCounter counter1 = new TabCounter();
                WriteToFile(writer, Utility.Text.Format("switch (propertyType)"));
                WriteNamespaceStart(writer);
                {
                    using TabCounter counter2 = new TabCounter();
                    foreach (var propertyInfo in componentType.GetFields())
                    {
                        Attribute[] propertyAttrs = Attribute.GetCustomAttributes(propertyInfo, typeof(GalaxyCombatPropertyAttribute));
                        if (propertyAttrs.Length == 0)
                            continue;

                        Debug.Assert(propertyAttrs.Length == 1);
                        var property = propertyAttrs[0] as GalaxyCombatPropertyAttribute;
                        if (property == null || propertyInfo.FieldType != typeof(IntVariable))
                            continue;

                        WriteToFile(writer, Utility.Text.Format("case {0}Enum.{1}:", componentAttribute.CompName, propertyInfo.Name));
                        using TabCounter counter3 = new TabCounter();
                        WriteToFile(writer, Utility.Text.Format("{0} = value;", propertyInfo.Name));
                        WriteToFile(writer, Utility.Text.Format("return;"));
                    }

                    WriteToFile(writer, Utility.Text.Format("default:"));
                    using TabCounter counter4 = new TabCounter();
                    WriteToFile(writer, Utility.Text.Format("Debug.Assert(false);"));
                    WriteToFile(writer, Utility.Text.Format("return;"));
                }
                WriteNamespaceEnd(writer);
            }
            WriteNamespaceEnd(writer);

            //SetProperty float value generator
            WriteToFile(writer, Utility.Text.Format("public void SetProperty({0}Enum propertyType, float value)", componentAttribute.CompName));
            WriteNamespaceStart(writer);
            {
                using TabCounter counter1 = new TabCounter();
                WriteToFile(writer, Utility.Text.Format("switch (propertyType)"));
                WriteNamespaceStart(writer);
                {
                    using TabCounter counter2 = new TabCounter();
                    foreach (var propertyInfo in componentType.GetFields())
                    {
                        Attribute[] propertyAttrs = Attribute.GetCustomAttributes(propertyInfo, typeof(GalaxyCombatPropertyAttribute));
                        if (propertyAttrs.Length == 0)
                            continue;

                        Debug.Assert(propertyAttrs.Length == 1);
                        var property = propertyAttrs[0] as GalaxyCombatPropertyAttribute;
                        if (property == null || propertyInfo.FieldType != typeof(FloatVariable))
                            continue;

                        WriteToFile(writer, Utility.Text.Format("case {0}Enum.{1}:", componentAttribute.CompName, propertyInfo.Name));
                        using TabCounter counter3 = new TabCounter();
                        WriteToFile(writer, Utility.Text.Format("{0} = value;", propertyInfo.Name));
                        WriteToFile(writer, Utility.Text.Format("return;"));
                    }

                    WriteToFile(writer, Utility.Text.Format("default:"));
                    using TabCounter counter4 = new TabCounter();
                    WriteToFile(writer, Utility.Text.Format("Debug.Assert(false);"));
                    WriteToFile(writer, Utility.Text.Format("return;"));
                }
                WriteNamespaceEnd(writer);
            }
            WriteNamespaceEnd(writer);

            //GetProperty int value generator
            WriteToFile(writer, Utility.Text.Format("public void GetProperty({0}Enum propertyType, ref int value)", componentAttribute.CompName));
            WriteNamespaceStart(writer);
            {
                using TabCounter counter1 = new TabCounter();
                WriteToFile(writer, Utility.Text.Format("switch (propertyType)"));
                WriteNamespaceStart(writer);
                {
                    using TabCounter counter2 = new TabCounter();
                    foreach (var propertyInfo in componentType.GetFields())
                    {
                        Attribute[] propertyAttrs = Attribute.GetCustomAttributes(propertyInfo, typeof(GalaxyCombatPropertyAttribute));
                        if (propertyAttrs.Length == 0)
                            continue;

                        Debug.Assert(propertyAttrs.Length == 1);
                        var property = propertyAttrs[0] as GalaxyCombatPropertyAttribute;
                        if (property == null || propertyInfo.FieldType != typeof(IntVariable))
                            continue;

                        WriteToFile(writer, Utility.Text.Format("case {0}Enum.{1}:", componentAttribute.CompName, propertyInfo.Name));
                        using TabCounter counter3 = new TabCounter();
                        WriteToFile(writer, Utility.Text.Format("value = {0};", propertyInfo.Name));
                        WriteToFile(writer, Utility.Text.Format("return;"));
                    }

                    WriteToFile(writer, Utility.Text.Format("default:"));
                    using TabCounter counter4 = new TabCounter();
                    WriteToFile(writer, Utility.Text.Format("Debug.Assert(false);"));
                    WriteToFile(writer, Utility.Text.Format("return;"));
                }
                WriteNamespaceEnd(writer);
            }
            WriteNamespaceEnd(writer);

            //GetProperty float value generator
            WriteToFile(writer, Utility.Text.Format("public void GetProperty({0}Enum propertyType, ref float value)", componentAttribute.CompName));
            WriteNamespaceStart(writer);
            {
                using TabCounter counter1 = new TabCounter();
                WriteToFile(writer, Utility.Text.Format("switch (propertyType)"));
                WriteNamespaceStart(writer);
                {
                    using TabCounter counter2 = new TabCounter();
                    foreach (var propertyInfo in componentType.GetFields())
                    {
                        Attribute[] propertyAttrs = Attribute.GetCustomAttributes(propertyInfo, typeof(GalaxyCombatPropertyAttribute));
                        if (propertyAttrs.Length == 0)
                            continue;

                        Debug.Assert(propertyAttrs.Length == 1);
                        var property = propertyAttrs[0] as GalaxyCombatPropertyAttribute;
                        if (property == null || propertyInfo.FieldType != typeof(FloatVariable))
                            continue;

                        WriteToFile(writer, Utility.Text.Format("case {0}Enum.{1}:", componentAttribute.CompName, propertyInfo.Name));
                        using TabCounter counter3 = new TabCounter();
                        WriteToFile(writer, Utility.Text.Format("value = {0};", propertyInfo.Name));
                        WriteToFile(writer, Utility.Text.Format("return;"));
                    }

                    WriteToFile(writer, Utility.Text.Format("default:"));
                    using TabCounter counter4 = new TabCounter();
                    WriteToFile(writer, Utility.Text.Format("Debug.Assert(false);"));
                    WriteToFile(writer, Utility.Text.Format("return;"));
                }
                WriteNamespaceEnd(writer);
            }
            WriteNamespaceEnd(writer);
        }

        private static void GeneratorForGalaxyCombatProxy(StreamWriter writer, Type componentType)
        {
            bool needGenerate = false;
            foreach (var propertyInfo in componentType.GetFields())
            {
                Attribute[] propertyAttrs = Attribute.GetCustomAttributes(propertyInfo, typeof(GalaxyCombatPropertyAttribute));
                if (propertyAttrs.Length == 0)
                    continue;

                Debug.Assert(propertyAttrs.Length == 1);
                var property = propertyAttrs[0] as GalaxyCombatPropertyAttribute;
                if (property == null)
                    Debug.Assert(false);

                needGenerate = true;
                break;
            }

            if (!needGenerate)
                return;

            Attribute[] classAttrs = Attribute.GetCustomAttributes(componentType, typeof(GalaxyComponentAttribute));
            Debug.Assert(classAttrs.Length == 1);
            GalaxyComponentAttribute componentAttribute = classAttrs[0] as GalaxyComponentAttribute;
            Debug.Assert(componentAttribute != null);

            //SetProperty int value generator
            WriteToFile(writer, Utility.Text.Format("public void SetProperty({0}Enum propertyType, int value)", componentAttribute.CompName));
            WriteNamespaceStart(writer);
            {
                using TabCounter counter = new TabCounter();
                WriteToFile(writer, Utility.Text.Format("m_Data.SetProperty(propertyType, value);"));
            }
            WriteNamespaceEnd(writer);

            //SetProperty float value generator
            WriteToFile(writer, Utility.Text.Format("public void SetProperty({0}Enum propertyType, float value)", componentAttribute.CompName));
            WriteNamespaceStart(writer);
            {
                using TabCounter counter = new TabCounter();
                WriteToFile(writer, Utility.Text.Format("m_Data.SetProperty(propertyType, value);"));
            }
            WriteNamespaceEnd(writer);

            //GetProperty int value generator
            WriteToFile(writer, Utility.Text.Format("public void GetProperty({0}Enum propertyType, ref int value)", componentAttribute.CompName));
            WriteNamespaceStart(writer);
            {
                using TabCounter counter = new TabCounter();
                WriteToFile(writer, Utility.Text.Format("m_Data.GetProperty(propertyType, ref value);"));
            }
            WriteNamespaceEnd(writer);

            //GetProperty float value generator
            WriteToFile(writer, Utility.Text.Format("public void GetProperty({0}Enum propertyType, ref float value)", componentAttribute.CompName));
            WriteNamespaceStart(writer);
            {
                using TabCounter counter = new TabCounter();
                WriteToFile(writer, Utility.Text.Format("m_Data.GetProperty(propertyType, ref value);"));
            }
            WriteNamespaceEnd(writer);
        }

        private static void GeneratorForGalaxyCombatExtend(StreamWriter writer, Type componentType)
        {
            bool needGenerate = false;
            foreach (var propertyInfo in componentType.GetFields())
            {
                Attribute[] propertyAttrs = Attribute.GetCustomAttributes(propertyInfo, typeof(GalaxyCombatPropertyAttribute));
                if (propertyAttrs.Length == 0)
                    continue;

                Debug.Assert(propertyAttrs.Length == 1);
                var property = propertyAttrs[0] as GalaxyCombatPropertyAttribute;
                if (property == null)
                    Debug.Assert(false);

                needGenerate = true;
                break;
            }

            if (!needGenerate)
                return;

            Attribute[] classAttrs = Attribute.GetCustomAttributes(componentType, typeof(GalaxyComponentAttribute));
            Debug.Assert(classAttrs.Length == 1);
            GalaxyComponentAttribute componentAttribute = classAttrs[0] as GalaxyComponentAttribute;
            Debug.Assert(componentAttribute != null);

            //Enum generator
            WriteToFile(writer, Utility.Text.Format("public enum {0}Enum", componentAttribute.CompName));
            WriteNamespaceStart(writer);
            {
                using TabCounter counter = new TabCounter();
                foreach (var propertyInfo in componentType.GetFields())
                {
                    Attribute[] propertyAttrs = Attribute.GetCustomAttributes(propertyInfo, typeof(GalaxyCombatPropertyAttribute));
                    if (propertyAttrs.Length == 0)
                        continue;

                    Debug.Assert(propertyAttrs.Length == 1);
                    var property = propertyAttrs[0] as GalaxyCombatPropertyAttribute;
                    if (property == null)
                        continue;

                    WriteToFile(writer, Utility.Text.Format("{0},", propertyInfo.Name));
                }
            }
            WriteNamespaceEnd(writer);

            //Generator helper class
            WriteToFile(writer, Utility.Text.Format("public class {0}Helper", componentAttribute.CompName));
            WriteNamespaceStart(writer);
            {
                using TabCounter counter = new TabCounter();
                Dictionary<Type, List<string>> propertyTypeNameDict = new Dictionary<Type, List<string>>();
                foreach (var propertyInfo in componentType.GetFields())
                {
                    Attribute[] propertyAttrs = Attribute.GetCustomAttributes(propertyInfo, typeof(GalaxyCombatPropertyAttribute));
                    if (propertyAttrs.Length == 0)
                        continue;

                    Debug.Assert(propertyAttrs.Length == 1);
                    var property = propertyAttrs[0] as GalaxyCombatPropertyAttribute;
                    if (property == null)
                        continue;

                    if (propertyInfo.FieldType != typeof(IntVariable) && propertyInfo.FieldType != typeof(FloatVariable))
                    {
                        //only support int & float
                        throw new NotImplementedException();
                    }

                    if (!propertyTypeNameDict.ContainsKey(propertyInfo.FieldType))
                    {
                        propertyTypeNameDict.Add(propertyInfo.FieldType, new List<string>());
                    }
                    propertyTypeNameDict[propertyInfo.FieldType].Add(propertyInfo.Name);
                }

                WriteToFile(writer, Utility.Text.Format("public static string GetAttributeStringId({0}Enum attrId)", componentAttribute.CompName));
                WriteNamespaceStart(writer);
                {
                    using TabCounter counter1 = new TabCounter();
                    WriteToFile(writer, Utility.Text.Format("switch (attrId)"));
                    WriteNamespaceStart(writer);
                    {
                        using TabCounter counter2 = new TabCounter();
                        if (propertyTypeNameDict.ContainsKey(typeof(IntVariable)))
                        {
                            foreach (var typeName in propertyTypeNameDict[typeof(IntVariable)])
                            {
                                WriteToFile(writer, Utility.Text.Format("case {0}Enum.{1}:", componentAttribute.CompName, typeName));
                                using TabCounter counter3 = new TabCounter();
                                {
                                    WriteToFile(writer, Utility.Text.Format("return \"{0}\";", typeName));
                                }
                            }
                        }
                        if (propertyTypeNameDict.ContainsKey(typeof(FloatVariable)))
                        {
                            foreach (var typeName in propertyTypeNameDict[typeof(FloatVariable)])
                            {
                                WriteToFile(writer, Utility.Text.Format("case {0}Enum.{1}:", componentAttribute.CompName, typeName));
                                using TabCounter counter3 = new TabCounter();
                                {
                                    WriteToFile(writer, Utility.Text.Format("return \"{0}\";", typeName));
                                }
                            }
                        }

                        {
                            WriteToFile(writer, "default: ");
                            using TabCounter counter3 = new TabCounter();
                            WriteToFile(writer, "return null;");
                        }
                    }
                    WriteNamespaceEnd(writer);
                }
                WriteNamespaceEnd(writer);

                WriteToFile(writer, Utility.Text.Format("public static bool IsIntAttribute({0}Enum attrId)", componentAttribute.CompName));
                WriteNamespaceStart(writer);
                {
                    using TabCounter counter1 = new TabCounter();
                    if (propertyTypeNameDict.ContainsKey(typeof(IntVariable)))
                    {
                        foreach (var typeName in propertyTypeNameDict[typeof(IntVariable)])
                        {
                            WriteToFile(writer, Utility.Text.Format("if ({0}Enum.{1} == attrId) return true;", componentAttribute.CompName, typeName));
                        }
                    }
                    WriteToFile(writer, Utility.Text.Format("return false;"));
                }
                WriteNamespaceEnd(writer);

                WriteToFile(writer, Utility.Text.Format("public static bool IsFloatAttribute({0}Enum attrId)", componentAttribute.CompName));
                WriteNamespaceStart(writer);
                {
                    using TabCounter counter1 = new TabCounter();
                    if (propertyTypeNameDict.ContainsKey(typeof(FloatVariable)))
                    {
                        foreach (var typeName in propertyTypeNameDict[typeof(FloatVariable)])
                        {
                            WriteToFile(writer, Utility.Text.Format("if ({0}Enum.{1} == attrId) return true;", componentAttribute.CompName, typeName));
                        }
                    }
                    WriteToFile(writer, Utility.Text.Format("return false;"));
                }
                WriteNamespaceEnd(writer);

            }
            WriteNamespaceEnd(writer);
        }

        #endregion

        #region Helper

        private static void GeneratorComponentHelper(StreamWriter writer, Dictionary<string, List<Type>> customComponents)
        {
            // helper
            WriteToFile(writer, Utility.Text.Format("public class ComponentHelper"));
            WriteNamespaceStart(writer);
            {
                using TabCounter counter1 = new TabCounter();
                //Component type to enum
                WriteToFile(writer, Utility.Text.Format("public static CompType GetCompEnumByType(Type type)"));
                WriteNamespaceStart(writer);
                {
                    using TabCounter counter2 = new TabCounter();
                    foreach (var item in customComponents)
                    {
                        foreach (var componentType in item.Value)
                        {
                            try
                            {
                                Attribute[] classAttrs = Attribute.GetCustomAttributes(componentType, typeof(GalaxyComponentAttribute));
                                Debug.Assert(classAttrs.Length == 1);
                                GalaxyComponentAttribute componentAttribute = classAttrs[0] as GalaxyComponentAttribute;
                                Debug.Assert(componentAttribute != null);
                                string componentFullName = Utility.Text.Format("{0}.{1}", componentType.Namespace, componentAttribute.CompName);

                                WriteToFile(writer, Utility.Text.Format("if (type == typeof({0}) || type == typeof({0}Proxy))", componentFullName));
                                WriteToFile(writer, Utility.Text.Format("\treturn CompType.{0};", componentAttribute.RealCompType.ToString()));
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

                //component
                WriteToFile(writer, Utility.Text.Format("public static ComponentBase CreateInstance(CompType compType)"));
                WriteNamespaceStart(writer);
                {
                    using TabCounter counter2 = new TabCounter();
                    WriteToFile(writer, "switch (compType)");
                    WriteNamespaceStart(writer);
                    {
                        using TabCounter counter3 = new TabCounter();
                        foreach (var item in customComponents)
                        {
                            foreach (var componentType in item.Value)
                            {
                                try
                                {
                                    Attribute[] classAttrs = Attribute.GetCustomAttributes(componentType, typeof(GalaxyComponentAttribute));
                                    Debug.Assert(classAttrs.Length == 1);
                                    GalaxyComponentAttribute componentAttribute = classAttrs[0] as GalaxyComponentAttribute;
                                    Debug.Assert(componentAttribute != null);
                                    string componentFullName = Utility.Text.Format("{0}.{1}", componentType.Namespace, componentAttribute.CompName);

                                    WriteToFile(writer, Utility.Text.Format("case CompType.{0}:", componentAttribute.RealCompType.ToString()));
                                    WriteToFile(writer, Utility.Text.Format("\treturn new {0}();", componentFullName));
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                            }
                        }

                        WriteToFile(writer, Utility.Text.Format("default:"));
                        WriteToFile(writer, Utility.Text.Format("\tthrow new NotImplementedException();"));
                    }
                    WriteNamespaceEnd(writer);
                }
                WriteNamespaceEnd(writer);

                //copy component
                WriteToFile(writer, Utility.Text.Format("public static ComponentBase CreateInstance(CompType compType, ComponentBase component)"));
                WriteNamespaceStart(writer);
                {
                    using TabCounter counter2 = new TabCounter();
                    WriteToFile(writer, "switch (compType)");
                    WriteNamespaceStart(writer);
                    {
                        using TabCounter counter3 = new TabCounter();
                        foreach (var item in customComponents)
                        {
                            foreach (var componentType in item.Value)
                            {
                                try
                                {
                                    Attribute[] classAttrs = Attribute.GetCustomAttributes(componentType, typeof(GalaxyComponentAttribute));
                                    Debug.Assert(classAttrs.Length == 1);
                                    GalaxyComponentAttribute componentAttribute = classAttrs[0] as GalaxyComponentAttribute;
                                    Debug.Assert(componentAttribute != null);
                                    string componentFullName = Utility.Text.Format("{0}.{1}", componentType.Namespace, componentAttribute.CompName);

                                    WriteToFile(writer, Utility.Text.Format("case CompType.{0}:", componentAttribute.RealCompType.ToString()));
                                    WriteNamespaceStart(writer);
                                    {
                                        using TabCounter counter4 = new TabCounter();
                                        WriteToFile(writer, Utility.Text.Format("{0} data = component as {0};", componentFullName));
                                        WriteToFile(writer, Utility.Text.Format("Debug.Assert(data != null);"));
                                        WriteToFile(writer, Utility.Text.Format("return new {0}(data);", componentFullName));
                                    }
                                    WriteNamespaceEnd(writer);
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                            }
                        }

                        WriteToFile(writer, Utility.Text.Format("default:"));
                        WriteToFile(writer, Utility.Text.Format("\tthrow new NotImplementedException();"));
                    }
                    WriteNamespaceEnd(writer);
                }
                WriteNamespaceEnd(writer);

                //component proxy
                WriteToFile(writer, Utility.Text.Format("public static ComponentProxy CreateProxyInstance(CompType compType, ComponentBase component, AccessType accessType)"));
                WriteNamespaceStart(writer);
                {
                    using TabCounter counter2 = new TabCounter();
                    WriteToFile(writer, "switch (compType)");
                    WriteNamespaceStart(writer);
                    {
                        using TabCounter counter3 = new TabCounter();
                        foreach (var item in customComponents)
                        {
                            foreach (var componentType in item.Value)
                            {
                                try
                                {
                                    Attribute[] classAttrs = Attribute.GetCustomAttributes(componentType, typeof(GalaxyComponentAttribute));
                                    Debug.Assert(classAttrs.Length == 1);
                                    GalaxyComponentAttribute componentAttribute = classAttrs[0] as GalaxyComponentAttribute;
                                    Debug.Assert(componentAttribute != null);
                                    string componentFullName = Utility.Text.Format("{0}.{1}", componentType.Namespace, componentAttribute.CompName);

                                    WriteToFile(writer, Utility.Text.Format("case CompType.{0}:", componentAttribute.RealCompType.ToString()));
                                    WriteNamespaceStart(writer);
                                    {
                                        using TabCounter counter4 = new TabCounter();
                                        WriteToFile(writer, Utility.Text.Format("{0} data = component as {0};", componentFullName));
                                        WriteToFile(writer, Utility.Text.Format("Debug.Assert(data != null);"));
                                        WriteToFile(writer, Utility.Text.Format("return new {0}Proxy(data, accessType);", componentFullName));
                                    }
                                    WriteNamespaceEnd(writer);
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                            }
                        }

                        WriteToFile(writer, Utility.Text.Format("default:"));
                        WriteToFile(writer, Utility.Text.Format("\tthrow new NotImplementedException();"));
                    }
                    WriteNamespaceEnd(writer);
                }
                WriteNamespaceEnd(writer);
            }
            WriteNamespaceEnd(writer);
        }

        #endregion
    }
#pragma warning restore CS0168
}
