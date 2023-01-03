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

namespace Galaxy.Mold
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GalaxyCommandAttribute : Attribute
    {
        public GalaxyCommandAttribute(CommandType commandType, SystemType[] systems, CompType[] rwComps, CompType[] roComps)
        {
            m_commandType = commandType;
            m_holderTypeDefine = new HolderTypeDefine(systems, rwComps, roComps);
        }

        private CommandType m_commandType;
        public CommandType CommandType => m_commandType;
        private HolderTypeDefine m_holderTypeDefine;
        public HolderTypeDefine HolderDefine => m_holderTypeDefine;
    }
#pragma warning disable CS0168
    public class CommandGenerator : GeneratorBase
    {
        private const string OutputExtendFileName = ".Proxy.cs";
        private const string HeaderImport = @"// auto generate file (mold)
using Galaxy.Data;
using Galaxy.Entities;
using Galaxy.Command;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
";

        public static void Generator()
        {
            string filesPath = UnityConst.DCSScriptsPath + @"GExtend\Command\CommandProxy\";
            string[] NamespaceList = GetAllNamespaces();
            Dictionary<string, List<Type>> customCommands = GetAllCustomCommand(NamespaceList);

            //customType.FullName
            foreach (var item in customCommands)
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
                            foreach (var command in item.Value)
                            {
                                GeneratorCommandProxy(writer, command);
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

            //command manager
            {
                string namespaceStr = "Galaxy.Command";
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
                            GeneratorCommandManagerProxy(writer, customCommands);
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

        private static Dictionary<string, List<Type>> GetAllCustomCommand(string[] namespaceList)
        {
            Dictionary<string, List<Type>> customCommands = new Dictionary<string, List<Type>>();
            foreach (var namespaceStr in namespaceList)
            {
                var result = from customType in Assembly.GetExecutingAssembly().GetTypes() where customType.IsClass && namespaceStr == customType.Namespace select customType;
                foreach (var customType in result.ToList())
                {
                    Attribute[] classAttrs = Attribute.GetCustomAttributes(customType, typeof(GalaxyCommandAttribute));
                    if (classAttrs.Length == 0)
                        continue;

                    if (!customCommands.ContainsKey(namespaceStr))
                        customCommands.Add(namespaceStr, new List<Type>());
                    customCommands[namespaceStr].Add(customType);
                }
            }
            return customCommands;
        }

        private static void GeneratorCommandProxy(StreamWriter writer, Type commandType)
        {
            try
            {
                Attribute[] classAttrs = Attribute.GetCustomAttributes(commandType, typeof(GalaxyCommandAttribute));
                Debug.Assert(classAttrs.Length == 1);
                GalaxyCommandAttribute commandAttribute = classAttrs[0] as GalaxyCommandAttribute;
                Debug.Assert(commandAttribute != null);

                // class
                WriteToFile(writer, Utility.Text.Format("public partial class {0}", commandType.Name));
                WriteNamespaceStart(writer);
                {
                    using TabCounter counter1 = new TabCounter();

                    WriteToFile(writer, Utility.Text.Format("public override CommandType GetCommandType()"));
                    WriteNamespaceStart(writer);
                    {
                        using TabCounter counter2 = new TabCounter();
                        WriteToFile(writer, Utility.Text.Format("return CommandType.{0};", commandAttribute.CommandType.ToString()));
                    }
                    WriteNamespaceEnd(writer);
                    WriteNewLine(writer);

                    // HolderTypeDefine
                    WriteToFile(writer, Utility.Text.Format("protected static HolderTypeDefine m_holderTypeDefine = new HolderTypeDefine("));
                    {
                        using TabCounter counter2 = new TabCounter();
                        string systemTypeStr = "{";
                        foreach (var item in commandAttribute.HolderDefine.m_DependSystemTypes)
                        {
                            systemTypeStr += "SystemType.";
                            systemTypeStr += item.ToString();
                            systemTypeStr += ", ";
                        }
                        systemTypeStr += "}";
                        WriteToFile(writer, Utility.Text.Format("new SystemType[] {0},", systemTypeStr));

                        string rwTypeStr = "{";
                        foreach (var item in commandAttribute.HolderDefine.m_RWCompTypes)
                        {
                            rwTypeStr += "CompType.";
                            rwTypeStr += item.ToString();
                            rwTypeStr += ", ";
                        }
                        rwTypeStr += "}";
                        WriteToFile(writer, Utility.Text.Format("new CompType[] {0},", rwTypeStr));

                        string roTypeStr = "{";
                        foreach (var item in commandAttribute.HolderDefine.m_ROCompTypes)
                        {
                            roTypeStr += "CompType.";
                            roTypeStr += item.ToString();
                            roTypeStr += ", ";
                        }
                        roTypeStr += "}";
                        WriteToFile(writer, Utility.Text.Format("new CompType[] {0}", roTypeStr));

                        WriteToFile(writer, Utility.Text.Format(");"));
                    }
                    WriteNewLine(writer);

                    WriteToFile(writer, Utility.Text.Format("public override HolderTypeDefine GetHolderTypeDefine()"));
                    WriteNamespaceStart(writer);
                    {
                        using TabCounter counter2 = new TabCounter();
                        WriteToFile(writer, Utility.Text.Format("return m_holderTypeDefine;"));
                    }
                    WriteNamespaceEnd(writer);
                }
                WriteNamespaceEnd(writer);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void GeneratorCommandManagerProxy(StreamWriter writer, Dictionary<string, List<Type>> customCommands)
        {
            // helper
            WriteToFile(writer, Utility.Text.Format("public class CommandHelper"));
            WriteNamespaceStart(writer);
            {
                using TabCounter counter1 = new TabCounter();
                WriteToFile(writer, Utility.Text.Format("public static Dictionary<CommandType, CommandBase> GetAllCommands()"));
                WriteNamespaceStart(writer);
                {
                    using TabCounter counter2 = new TabCounter();
                    WriteToFile(writer, "return new Dictionary<CommandType, CommandBase> {");
                    {
                        using TabCounter counter3 = new TabCounter();
                        foreach (var item in customCommands)
                        {
                            foreach (var commandType in item.Value)
                            {
                                try
                                {
                                    Attribute[] classAttrs = Attribute.GetCustomAttributes(commandType, typeof(GalaxyCommandAttribute));
                                    Debug.Assert(classAttrs.Length == 1);
                                    GalaxyCommandAttribute commandAttribute = classAttrs[0] as GalaxyCommandAttribute;
                                    Debug.Assert(commandAttribute != null);
                                    WriteToFile(writer, "{" + Utility.Text.Format("CommandType.{0}, new {1}()", commandAttribute.CommandType.ToString(), commandType.FullName) + " },");
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                            }
                        }
                    }
                    WriteToFile(writer, "};");
                }
                WriteNamespaceEnd(writer);
            }
            WriteNamespaceEnd(writer);
        }
    }
#pragma warning restore CS0168
}
