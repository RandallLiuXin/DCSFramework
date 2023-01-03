using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using Galaxy.Entities;

namespace Galaxy.Mold
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GalaxySystemAttribute : Attribute
    {
        public GalaxySystemAttribute(SystemType systemType)
        {
            m_systemType = systemType;
        }

        private SystemType m_systemType;
        public SystemType SystemType => m_systemType;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class GalaxyMethodAttribute : Attribute
    {
        public GalaxyMethodAttribute()
        {
        }
    }

#pragma warning disable CS0168
    public sealed class SystemGenerator : GeneratorBase
    {
        private const string OutputExtendFileName = ".Proxy.cs";
        private const string HeaderImport = @"// auto generate file (mold)
using Galaxy.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
";

        public static void Generator()
        {
            string filesPath = UnityConst.DCSScriptsPath + @"GExtend\System\SystemProxy\";
            string[] NamespaceList = GetAllNamespaces();
            Dictionary<string, List<Type>> customSystems = GetAllCustomSystem(NamespaceList);

            //customType.FullName
            foreach (var item in customSystems)
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
                            foreach (var system in item.Value)
                            {
                                GeneratorSystemProxy(writer, system);
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

            //system manager
            {
                string namespaceStr = "Galaxy.Entities";
                string outputName = filesPath + "Galaxy.SystemHelper.cs";
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
                            GeneratorSystemHelper(writer, customSystems);
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

        private static Dictionary<string, List<Type>> GetAllCustomSystem(string[] namespaceList)
        {
            Dictionary<string, List<Type>> customSystems = new Dictionary<string, List<Type>>();
            foreach (var namespaceStr in namespaceList)
            {
                var result = from customType in Assembly.GetExecutingAssembly().GetTypes() where customType.IsClass && namespaceStr == customType.Namespace select customType;
                foreach (var customType in result.ToList())
                {
                    Attribute[] classAttrs = Attribute.GetCustomAttributes(customType, typeof(GalaxySystemAttribute));
                    if (classAttrs.Length == 0)
                        continue;

                    if (!customSystems.ContainsKey(namespaceStr))
                        customSystems.Add(namespaceStr, new List<Type>());
                    customSystems[namespaceStr].Add(customType);
                }
            }
            return customSystems;
        }

        private static void GeneratorSystemProxy(StreamWriter writer, Type systemType)
        {
            try
            {
                // class
                WriteToFile(writer, Utility.Text.Format("public class {0}Proxy : GalaxySystemProxy", systemType.Name));
                WriteNamespaceStart(writer);
                {
                    using TabCounter counter1 = new TabCounter();
                    // constructor & abstract
                    WriteToFile(writer, Utility.Text.Format("protected {0} m_System;", systemType.FullName));
                    WriteToFile(writer, Utility.Text.Format("public {0}Proxy(GalaxySystem system)", systemType.Name));
                    WriteNamespaceStart(writer);
                    {
                        using TabCounter counter2 = new TabCounter();
                        WriteToFile(writer, Utility.Text.Format("m_System = system as {0};", systemType.FullName));
                        WriteToFile(writer, Utility.Text.Format("Debug.Assert(m_System != null);"));
                    }
                    WriteNamespaceEnd(writer);
                    WriteNewLine(writer);

                    WriteToFile(writer, Utility.Text.Format("protected override GalaxySystem GetGalaxySystem()"));
                    WriteNamespaceStart(writer);
                    {
                        using TabCounter counter3 = new TabCounter();
                        WriteToFile(writer, Utility.Text.Format("return m_System;"));
                    }
                    WriteNamespaceEnd(writer);
                    WriteNewLine(writer);

                    // method
                    foreach (var methodInfo in systemType.GetMethods())
                    {
                        Attribute[] methodAttrs = Attribute.GetCustomAttributes(methodInfo, typeof(GalaxyMethodAttribute));
                        if (methodAttrs.Length == 0)
                            continue;

                        Debug.Assert(methodAttrs.Length == 1);
                        var method = methodAttrs[0];

                        WriteMethodToFile(writer, methodInfo);
                    }
                }
                WriteNamespaceEnd(writer);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void WriteMethodToFile(StreamWriter writer, MethodInfo methodInfo)
        {
            //UID
            {
                bool isFirstParam = true;
                string parameters = "";
                string parameterNames = "";
                foreach (ParameterInfo parameterInfo in methodInfo.GetParameters())
                {
                    if (isFirstParam)
                    {
                        parameters = "uint uid";
                        isFirstParam = false;
                        continue;
                    }

                    parameters += ", ";
                    if (parameterInfo.ParameterType.IsByRef)
                    {
                        parameters += Utility.Text.Format("ref {0} ", parameterInfo.ParameterType.FullName.Trim('&'));
                    }
                    else
                    {
                        parameters += Utility.Text.Format("{0} ", GetPropertyTypeString(parameterInfo.ParameterType));
                    }
                    parameters += parameterInfo.Name;

                    parameterNames += ", ";
                    if (parameterInfo.ParameterType.IsByRef)
                    {
                        parameterNames += "ref ";
                    }
                    parameterNames += parameterInfo.Name;
                }

                WriteToFile(writer, Utility.Text.Format("{0} {1}{2} {3}({4})",
                    methodInfo.IsPublic ? "public" : "private",
                    methodInfo.IsStatic ? "static " : methodInfo.IsVirtual ? "virtual " : "",
                    GetReturnTypeString(methodInfo.ReturnType),
                    methodInfo.Name,
                    parameters));
                WriteNamespaceStart(writer);
                {

                    using TabCounter counter = new TabCounter();
                    WriteToFile(writer, Utility.Text.Format("{0}m_System.{1}(GetHolder(uid){2});",
                        methodInfo.ReturnType != typeof(void) ? "return " : "",
                        methodInfo.Name,
                        parameterNames));
                }
                WriteNamespaceEnd(writer);
            }

            //HolderProxy
            {
                bool isFirstParam = true;
                string parameters = "";
                string parameterNames = "";
                foreach (ParameterInfo parameterInfo in methodInfo.GetParameters())
                {
                    // 除第一个参数外需要添加分隔符
                    if (!isFirstParam)
                        parameters += ", ";

                    if (parameterInfo.ParameterType.IsByRef)
                    {
                        parameters += Utility.Text.Format("ref {0} ", parameterInfo.ParameterType.FullName.Trim('&'));
                    }
                    else
                    {
                        parameters += Utility.Text.Format("{0} ", GetPropertyTypeString(parameterInfo.ParameterType));
                    }
                    parameters += parameterInfo.Name;

                    if (!isFirstParam)
                    {
                        parameterNames += ", ";
                        if (parameterInfo.ParameterType.IsByRef)
                        {
                            parameterNames += "ref ";
                        }
                        parameterNames += parameterInfo.Name;
                    }
                    isFirstParam = false;
                }

                WriteToFile(writer, Utility.Text.Format("{0} {1}{2} {3}({4})",
                    methodInfo.IsPublic ? "public" : "private",
                    methodInfo.IsStatic ? "static " : methodInfo.IsVirtual ? "virtual " : "",
                    GetReturnTypeString(methodInfo.ReturnType),
                    methodInfo.Name,
                    parameters));
                WriteNamespaceStart(writer);
                {

                    using TabCounter counter = new TabCounter();
                    WriteToFile(writer, Utility.Text.Format("{0}m_System.{1}(GetHolder(holder){2});",
                        methodInfo.ReturnType != typeof(void) ? "return " : "",
                        methodInfo.Name,
                        parameterNames));
                }
                WriteNamespaceEnd(writer);
            }
        }

        private static void GeneratorSystemHelper(StreamWriter writer, Dictionary<string, List<Type>> customSystems)
        {
            // helper
            WriteToFile(writer, Utility.Text.Format("public class SystemHelper"));
            WriteNamespaceStart(writer);
            {
                using TabCounter counter1 = new TabCounter();
                //System type to enum
                WriteToFile(writer, Utility.Text.Format("public static SystemType GetSystemEnumByType(Type type)"));
                WriteNamespaceStart(writer);
                {
                    using TabCounter counter2 = new TabCounter();
                    foreach (var item in customSystems)
                    {
                        foreach (var systemType in item.Value)
                        {
                            try
                            {
                                Attribute[] classAttrs = Attribute.GetCustomAttributes(systemType, typeof(GalaxySystemAttribute));
                                Debug.Assert(classAttrs.Length == 1);
                                GalaxySystemAttribute systemAttribute = classAttrs[0] as GalaxySystemAttribute;
                                Debug.Assert(systemAttribute != null);

                                WriteToFile(writer, Utility.Text.Format("if (type == typeof({0}) || type == typeof({0}Proxy))", systemType.FullName));
                                WriteToFile(writer, Utility.Text.Format("\treturn SystemType.{0};", systemAttribute.SystemType.ToString()));
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

                //system
                WriteToFile(writer, Utility.Text.Format("public static GalaxySystem CreateInstance(SystemType systemType)"));
                WriteNamespaceStart(writer);
                {
                    using TabCounter counter2 = new TabCounter();
                    WriteToFile(writer, "switch (systemType)");
                    WriteNamespaceStart(writer);
                    {
                        using TabCounter counter3 = new TabCounter();
                        foreach (var item in customSystems)
                        {
                            foreach (var systemType in item.Value)
                            {
                                try
                                {
                                    Attribute[] classAttrs = Attribute.GetCustomAttributes(systemType, typeof(GalaxySystemAttribute));
                                    Debug.Assert(classAttrs.Length == 1);
                                    GalaxySystemAttribute systemAttribute = classAttrs[0] as GalaxySystemAttribute;
                                    Debug.Assert(systemAttribute != null);

                                    WriteToFile(writer, Utility.Text.Format("case SystemType.{0}:", systemAttribute.SystemType.ToString()));
                                    WriteToFile(writer, Utility.Text.Format("\treturn new {0}();", systemType.FullName));
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

                //system proxy
                WriteToFile(writer, Utility.Text.Format("public static GalaxySystemProxy CreateProxyInstance(SystemType systemType, GalaxySystem system)"));
                WriteNamespaceStart(writer);
                {
                    using TabCounter counter2 = new TabCounter();
                    WriteToFile(writer, "switch (systemType)");
                    WriteNamespaceStart(writer);
                    {
                        using TabCounter counter3 = new TabCounter();
                        foreach (var item in customSystems)
                        {
                            foreach (var systemType in item.Value)
                            {
                                try
                                {
                                    Attribute[] classAttrs = Attribute.GetCustomAttributes(systemType, typeof(GalaxySystemAttribute));
                                    Debug.Assert(classAttrs.Length == 1);
                                    GalaxySystemAttribute systemAttribute = classAttrs[0] as GalaxySystemAttribute;
                                    Debug.Assert(systemAttribute != null);

                                    WriteToFile(writer, Utility.Text.Format("case SystemType.{0}:", systemAttribute.SystemType.ToString()));
                                    WriteToFile(writer, Utility.Text.Format("\treturn new {0}Proxy(system);", systemType.FullName));
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
    }
#pragma warning restore CS0168
}
