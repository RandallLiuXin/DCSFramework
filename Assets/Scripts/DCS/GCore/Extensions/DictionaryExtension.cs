using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Galaxy
{
    public static class DictionaryExtension
    {
        public static void ForceAddList<K, V, T>(this Dictionary<K, V> dict, K key, T value, bool acceptSame = false) where V : List<T>, new()
        {
            bool shouldNewAdd = false;
            if (dict.ContainsKey(key))
            {
                V list = dict[key];
                if (list == null)
                {
                    dict.Remove(key);
                    shouldNewAdd = true;
                }
                else
                {
                    if (acceptSame)
                    {
                        list.Add(value);
                    }
                    else if (!list.Contains(value))
                    {
                        list.Add(value);
                    }
                }
            }
            else
            {
                shouldNewAdd = true;
            }
            if (shouldNewAdd)
            {
                V list = new V();
                list.Add(value);
                dict.Add(key, list);
            }
        }
        public static void ForceAdd<K, V>(this Dictionary<K, V> dict, K key, V value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
        }
        public static void ForceRemove<K, V>(this Dictionary<K, V> dict, K key)
        {
            if (dict.ContainsKey(key))
            {
                dict.Remove(key);
            }
        }
        public static void SafeAdd<K, V>(this Dictionary<K, V> dict, K key, V value, bool errorOnDuplicate, string contextInformation)
        {
            if (dict.ContainsKey(key))
            {
                if (errorOnDuplicate)
                {
                    Debug.LogError(string.Concat(new object[]
                    {
                    "Error: Attempt to add a key \"",
                    key,
                    "\" to dictionary failed because it already existed. (CONTEXT: ",
                    contextInformation,
                    ")"
                    }).ToString());
                }
                return;
            }
            dict.Add(key, value);
        }
        public static V SafeGet<K, V>(this Dictionary<K, V> dict, K key, V defaultValue, bool errorOnNotFound, string contextInformation)
        {
            if (!dict.ContainsKey(key))
            {
                if (errorOnNotFound)
                {
                    Debug.LogError(string.Concat(new object[]
                    {
                    "Error: Attempt to get a key \"",
                    key,
                    "\" from dictionary failed because it doesn't exist. (CONTEXT: ",
                    contextInformation,
                    ")"
                    }).ToString());
                }
                return defaultValue;
            }
            return dict[key];
        }
        public static string PrintKeys<K, V>(this Dictionary<K, V> dict)
        {
            string text = "{";
            foreach (KeyValuePair<K, V> current in dict)
            {
                text = text + current.Key + ", ";
            }
            if (dict.Count > 0)
            {
                text = text.Substring(0, text.Length - 1);
            }
            text += "}";
            return text;
        }
        public static string PrintValues<K, V>(this Dictionary<K, V> dict)
        {
            string text = "{";
            foreach (KeyValuePair<K, V> current in dict)
            {
                text = text + current.Value + ", ";
            }
            if (dict.Count > 0)
            {
                text = text.Substring(0, text.Length - 1);
            }
            text += "}";
            return text;
        }
        public static string PrintKeyValuePairs<K, V>(this Dictionary<K, V> dict)
        {
            string text = "{";
            foreach (KeyValuePair<K, V> current in dict)
            {
                string text2 = text;
                text = string.Concat(new object[]
                {
                text2,
                current.Key,
                ":",
                current.Value,
                ", "
                });
            }
            if (dict.Count > 0)
            {
                text = text.Substring(0, text.Length - 1);
            }
            text += "}";
            return text;
        }
        public static Dictionary<string, object> MergeDictionaries(Dictionary<string, object> dictA, Dictionary<string, object> dictB)
        {
            return dictA.Concat(
                from kvp in dictB
                where !dictA.ContainsKey(kvp.Key)
                select kvp).ToDictionary((KeyValuePair<string, object> kvp) => kvp.Key, (KeyValuePair<string, object> kvp) => kvp.Value);
        }
        public static Dictionary<NK, NV> CastDictionary<NK, NV, OK, OV>(this Dictionary<OK, OV> dict) where NK : class where NV : class where OK : class where OV : class
        {
            Dictionary<NK, NV> dictionary = new Dictionary<NK, NV>();
            foreach (KeyValuePair<OK, OV> current in dict)
            {
                NK nK = current.Key as NK;
                NV nV = current.Key as NV;
                if (nK == null || (nV == null && current.Value != null))
                {
                    Debug.LogError("Cannot cast dictionary types");
                    return dictionary;
                }
                dictionary.Add(nK, nV);
            }
            return dictionary;
        }
        public static void AddKeyValuePair<K, V>(this Dictionary<K, V> dict, KeyValuePair<K, V> pair, bool errorOnDuplicate, string contextInformation)
        {
            if (dict.ContainsKey(pair.Key))
            {
                if (errorOnDuplicate)
                {
                    Debug.LogError(string.Concat(new object[]
                    {
                    "Error: Attempt to add a key value pair with key \"",
                    pair.Key,
                    "\" to dictionary failed because it already existed. (CONTEXT: ",
                    contextInformation,
                    ")"
                    }).ToString());
                }
                return;
            }
            dict.Add(pair.Key, pair.Value);
        }

        /// <summary>
        /// Increment counter at the key passed as argument. Dictionary is <TKey, Int> 
        /// Example:
        /// var animalQuantities = new Dictionary<string, int>();
        /// animalQuantities.IncrementAt("cat");
        /// animalQuantities.IncrementAt("cat");
        /// Console.WriteLine(animalQuantities["cat"]); // 2
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="index"></param>
        public static void IncrementAt<T>(this Dictionary<T, int> dictionary, T index)
        {
            int count = 0;

            dictionary.TryGetValue(index, out count);

            dictionary[index] = ++count;
        }

        /// <summary>
        /// Converts Enumeration type into a dictionary of names and values
        /// Example: 
        /// var dictionary = typeof(UriFormat).EnumToDictionary();
        /// </summary>
        /// <param name="t">Enum type</param>
        public static IDictionary<string, int> EnumToDictionary(this Type t)
        {
            if (t == null) throw new NullReferenceException();
            if (!t.IsEnum) throw new InvalidCastException("object is not an Enumeration");

            string[] names = Enum.GetNames(t);
            Array values = Enum.GetValues(t);

            return (from i in Enumerable.Range(0, names.Length)
                    select new { Key = names[i], Value = (int)values.GetValue(i) })
                        .ToDictionary(k => k.Key, k => k.Value);
        }

        /// <summary>
        /// Converts an enumeration of groupings into a Dictionary of those groupings.
        /// Dictionary<string,List<Product>> results = productList.GroupBy(product => product.Category).ToDictionary();
        /// </summary>
        /// <typeparam name="TKey">Key type of the grouping and dictionary.</typeparam>
        /// <typeparam name="TValue">Element type of the grouping and dictionary list.</typeparam>
        /// <param name="groupings">The enumeration of groupings from a GroupBy() clause.</param>
        /// <returns>A dictionary of groupings such that the key of the dictionary is TKey type and the value is List of TValue type.</returns>
        public static Dictionary<TKey, List<TValue>> ToDictionary<TKey, TValue>(this IEnumerable<IGrouping<TKey, TValue>> groupings)
        {
            return groupings.ToDictionary(group => group.Key, group => group.ToList());
        }
        
        /// <summary>
        /// Converts a hashtable to a generic ditionary
        /// Example:
        /// Hashtable t1 = new Hashtable();
        /// t1.Add("pi", Math.PI);
        /// t1.Add("e", Math.E);
        /// var col = t1.Upgrade<string, double>();
        /// </summary>
        /// <typeparam name="TKey">The type of the key in the hashtable</typeparam>
        /// <typeparam name="TValue">The type of the object in the hashtable</typeparam>
        /// <param name="t">The hashtable</param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> Upgrade<TKey, TValue>(this Hashtable t)
        {
            var dic = new Dictionary<TKey, TValue>();

            foreach (DictionaryEntry entry in t)
            {
                dic.Add((TKey)entry.Key, (TValue)entry.Value);
            }
            return dic;
        }

        /// <summary>
        /// dictionary值排序  默认为正序
        /// </summary>
        public static void DictionarySort<T, V>(this Dictionary<T, V> dict, bool isInverted = false) where V : System.IComparable
        {
            if (dict.Count <= 0)
                return;

            List<KeyValuePair<T, V>> lst = new List<KeyValuePair<T, V>>(dict);
            lst.Sort(delegate (KeyValuePair<T, V> s1, KeyValuePair<T, V> s2)
            {
                if (isInverted)
                {
                    return s2.Value.CompareTo(s1.Value);
                }
                return s1.Value.CompareTo(s2.Value);
            });
            dict.Clear();

            foreach (KeyValuePair<T, V> kvp in lst)
            {
                dict.Add(kvp.Key, kvp.Value);
            }
        }
    }
}

