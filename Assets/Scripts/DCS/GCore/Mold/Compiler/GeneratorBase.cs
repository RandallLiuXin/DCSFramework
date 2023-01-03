using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Reflection;

namespace Galaxy.Mold
{
    public class MoldBase
    {
        public MoldBase()
        {
        }
    }

    public class GeneratorBase
    {
        private const string NamespaceStart = "{";
        private const string NamespaceEnd = "}";

        protected sealed class TabCounter : IDisposable
        {
            public TabCounter()
            {
                ms_NumOfTab++;
            }

            ~TabCounter()
            {
            }

            public void Dispose()
            {
                ms_NumOfTab--;
            }
        }

        protected static uint ms_NumOfTab = 0;
        protected static string GetTabStr()
        {
            switch (ms_NumOfTab)    
            {
                case 0:
                    return "";
                case 1:
                    return "\t";
                case 2:
                    return "\t\t";
                case 3:
                    return "\t\t\t";
                case 4:
                    return "\t\t\t\t";
                case 5:
                    return "\t\t\t\t\t";
                case 6:
                    return "\t\t\t\t\t\t";
                case 7:
                    return "\t\t\t\t\t\t\t";
                default:
                    throw new NotImplementedException();
            }
        }

        protected static string[] GetAllNamespaces()
        {
            return new string[] {
                "Galaxy.Input",
                "Galaxy.Visual",
                "Galaxy.Movement",
                "Galaxy.State",
                "Galaxy.Property",
                "Galaxy.Command",
                "Galaxy.Test",
                "Galaxy.AI",
                "Galaxy.Level",
                "Galaxy.Entities",
                "Galaxy.Player",
                "Galaxy.UI",
            };
        }

        protected static string GetReturnTypeString(Type type)
        {
            if (type == typeof(void))
            {
                return "void";
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(HashSet<>))
            {
                Type argType = type.GetGenericArguments()[0];
                return Utility.Text.Format("HashSet<{0}>", GetGenericType(argType));
            }
            else if (type.IsGenericType)
            {
                return GetGenericType(type);
            }
            else
            {
                return type.FullName;
            }
        }

        protected static string GetPropertyReturnTypeString(Type type)
        {
            if (type.IsSubclassOf(typeof(Variable)))
            {
                return VariableHelper.GetVariableBaseType(type);
            }
            else
            {
                return GetPropertyTypeString(type);
            }
        }

        protected static string GetPropertyTypeString(Type type)
        {
            if (type.IsGenericType)
            {
                return GetGenericType(type);
            }
            else
            {
                return type.FullName;
            }
        }

        protected static string GetPropertyInitValueStr(Type type)
        {
            if (type.IsGenericType)
            {
                return Utility.Text.Format("new {0}()", GetGenericType(type));
            }
            else if (type == typeof(bool))
            {
                return "false";
            }
            else if (type == typeof(int)
                || type == typeof(uint)
                || type == typeof(ulong))
            {
                return "0";
            }
            else if (type == typeof(float))
            {
                return "0.0f";
            }
            else if (type == typeof(string))
            {
                return "\"\"";
            }
            else if (type == typeof(Vector3))
            {
                return "Vector3.zero";
            }
            else if (type == typeof(Quaternion))
            {
                return "Quaternion.identity";
            }
            else if (type.IsSubclassOf(typeof(Variable)))
            {
                return Utility.Text.Format("new {0}()", GetPropertyTypeString(type));
            }
            else if (type == typeof(GameObject))
            {
                return "null";
            }
            else
            {
                throw new NotImplementedException();
            }    
        }

        protected static string GetGenericType(Type type)
        {
            if (type.IsGenericType)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    Type keyType = type.GetGenericArguments()[0];
                    Type valueType = type.GetGenericArguments()[1];
                    return Utility.Text.Format("Dictionary<{0}, {1}>", GetGenericType(keyType), GetGenericType(valueType));
                }
                else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Type argType = type.GetGenericArguments()[0];
                    return Utility.Text.Format("List<{0}>", GetGenericType(argType));
                }
                else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(LinkedList<>))
                {
                    Type argType = type.GetGenericArguments()[0];
                    return Utility.Text.Format("LinkedList<{0}>", GetGenericType(argType));
                }
                else if (type.GetGenericTypeDefinition() == typeof(Tuple<,>))
                {
                    Type argType1 = type.GetGenericArguments()[0];
                    Type argType2 = type.GetGenericArguments()[1];
                    return Utility.Text.Format("Tuple<{0}, {1}>", GetGenericType(argType1), GetGenericType(argType2));
                }
                else if (type.GetGenericTypeDefinition() == typeof(Tuple<,,,>))
                {
                    Type argType1 = type.GetGenericArguments()[0];
                    Type argType2 = type.GetGenericArguments()[1];
                    Type argType3 = type.GetGenericArguments()[2];
                    Type argType4 = type.GetGenericArguments()[3];
                    return Utility.Text.Format("Tuple<{0}, {1}, {2}, {3}>", GetGenericType(argType1), GetGenericType(argType2), GetGenericType(argType3), GetGenericType(argType4));
                }
                else if (type.GetGenericTypeDefinition() == typeof(HashSet<>))
                {
                    Type argType = type.GetGenericArguments()[0];
                    return Utility.Text.Format("HashSet<{0}>", GetGenericType(argType));
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                return GetPropertyTypeString(type);
            }
        }

        protected static string GetPropertyValueStr(Type type, string propertyName)
        {
            if (type.IsSubclassOf(typeof(Variable)))
            {
                return Utility.Text.Format("m_{0}.Value", propertyName);
            }
            else
            {
                return Utility.Text.Format("m_{0}", propertyName);
            }
        }

        protected static void WriteNamespaceStart(StreamWriter writer)
        {
            WriteToFile(writer, NamespaceStart);
        }

        protected static void WriteNamespaceEnd(StreamWriter writer)
        {
            WriteToFile(writer, NamespaceEnd);
        }

        protected static void WriteNewLine(StreamWriter writer)
        {
            WriteToFile(writer, "");
        }

        protected static void WriteToFile(StreamWriter writer, string content)
        {
            content = Utility.Text.Format("{0}{1}", GetTabStr(), content);
            if (MoldMain.IsRelease)
            {
                writer.WriteLine(content);
            }
            else
            {
                Debug.Log(content);
            }
        }
    }
}
