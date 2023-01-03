using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Galaxy.Entities;
using Galaxy.Data;

namespace Galaxy.Mold
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GalaxyEntityAttribute : Attribute
    {
        public GalaxyEntityAttribute(EntityType entityType)
        {
            m_entityType = entityType;
        }

        private EntityType m_entityType;
        public EntityType EntityType => m_entityType;
    }
#pragma warning disable CS0168
    public class EntityGenerator : GeneratorBase
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
            string filesPath = UnityConst.DCSScriptsPath + @"GExtend\Entity\";
            string[] NamespaceList = GetAllNamespaces();
            Dictionary<string, List<Type>> customEntities = GetAllCustomEntities(NamespaceList);

            //entity manager
            {
                string namespaceStr = "Galaxy.Entities";
                string outputName = filesPath + "Galaxy.EntityHelper.cs";
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
                            GeneratorEntityHelper(writer, customEntities);
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

        private static Dictionary<string, List<Type>> GetAllCustomEntities(string[] namespaceList)
        {
            Dictionary<string, List<Type>> customEntities = new Dictionary<string, List<Type>>();
            foreach (var namespaceStr in namespaceList)
            {
                var result = from customType in Assembly.GetExecutingAssembly().GetTypes() where customType.IsClass && namespaceStr == customType.Namespace select customType;
                foreach (var customType in result.ToList())
                {
                    Attribute[] classAttrs = Attribute.GetCustomAttributes(customType, typeof(GalaxyEntityAttribute));
                    if (classAttrs.Length == 0)
                        continue;

                    if (!customEntities.ContainsKey(namespaceStr))
                        customEntities.Add(namespaceStr, new List<Type>());
                    customEntities[namespaceStr].Add(customType);
                }
            }
            return customEntities;
        }

        private static void GeneratorEntityHelper(StreamWriter writer, Dictionary<string, List<Type>> customEntities)
        {
            // helper
            WriteToFile(writer, Utility.Text.Format("public partial class EntityHelper"));
            WriteNamespaceStart(writer);
            {
                using TabCounter counter1 = new TabCounter();
                WriteToFile(writer, Utility.Text.Format("public static Entity CreateInstance(EntityType entityType, uint id, SystemType[] systems)"));
                WriteNamespaceStart(writer);
                {
                    using TabCounter counter2 = new TabCounter();
                    WriteToFile(writer, "switch (entityType)");
                    WriteNamespaceStart(writer);
                    {
                        using TabCounter counter3 = new TabCounter();
                        foreach (var item in customEntities)
                        {
                            foreach (var entityType in item.Value)
                            {
                                try
                                {
                                    Attribute[] classAttrs = Attribute.GetCustomAttributes(entityType, typeof(GalaxyEntityAttribute));
                                    Debug.Assert(classAttrs.Length == 1);
                                    GalaxyEntityAttribute entityAttribute = classAttrs[0] as GalaxyEntityAttribute;
                                    Debug.Assert(entityAttribute != null);

                                    WriteToFile(writer, Utility.Text.Format("case EntityType.{0}:", entityAttribute.EntityType.ToString()));
                                    WriteToFile(writer, Utility.Text.Format("\treturn new {0}(id, systems);", entityType.FullName));
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
