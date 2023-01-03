using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Galaxy
{
    public static class ListExtension
    {
        public static void SafeAdd<T>(this List<T> list, T value, bool errorOnDuplicate = false, string contextInformation = "")
        {
            if (list.Contains(value))
            {
                if (errorOnDuplicate)
                {
                    Debug.LogError(string.Concat(new object[]
                       {
                    "Error: Attempt to add a value \"",
                    value,
                    "\" to list failed because it already existed. (CONTEXT: ",
                    contextInformation,
                    ")"
                       }).ToString());
                }
                return;
            }
            list.Add(value);
        }

        public static void SafeRemove<T>(this List<T> list, T value, bool errorOnDuplicate = false, string contextInformation = "")
        {
            if (!list.Contains(value))
            {
                if (errorOnDuplicate)
                {
                    Debug.LogError(string.Concat(new object[]
                       {
                    "Error: Attempt to add a value \"",
                    value,
                    "\" to list failed because it already existed. (CONTEXT: ",
                    contextInformation,
                    ")"
                       }).ToString());
                }
                return;
            }
            list.Remove(value);
        }

        public static bool SafeContains<T>(this T[] source, T value)
        {
            if (source != null)
            {
                int count = source.Count();
                for (int i = 0; i < count; i++)
                {
                    if (source[i] != null && source[i].Equals(value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        public static bool None<T>(this IEnumerable<T> source)
        {
            return source.Any() == false;
        }

        public static bool None<T>(this IEnumerable<T> source, Func<T, bool> query)
        {
            return source.Any(query) == false;
        }

        public static bool Many<T>(this IEnumerable<T> source)
        {
            return source.Count() > 1;
        }

        public static bool Many<T>(this IEnumerable<T> source, Func<T, bool> query)
        {
            return source.Count(query) > 1;
        }

        public static bool OneOf<T>(this IEnumerable<T> source)
        {
            return source.Count() == 1;
        }

        public static bool OneOf<T>(this IEnumerable<T> source, Func<T, bool> query)
        {
            return source.Count(query) == 1;
        }

        public static bool XOf<T>(this IEnumerable<T> source, int count)
        {
            return source.Count() == count;
        }

        public static bool XOf<T>(this IEnumerable<T> source, Func<T, bool> query, int count)
        {
            return source.Count(query) == count;
        }

        public static bool Replace<T>(this IList<T> thisList, int position, T item)
        {
            if (position > thisList.Count - 1)
                return false;

            thisList.RemoveAt(position);
            thisList.Insert(position, item);
            return true;
        }

        /// <summary>
        /// Converts an arraylist to a generic List
        /// </summary>
        /// <typeparam name="T">The type of the elements in the arraylist</typeparam>
        /// <param name="l">The arraylist</param>
        /// <returns></returns>
        public static List<T> Upgrade<T>(this ArrayList l)
        {
            var list = new List<T>();

            foreach (T entry in l)
            {
                list.Add(entry);
            }

            return list;
        }

        /// <summary>
        /// Sorteia randomicamente um item de um IList e o retorna
        /// </summary>
        /// <typeparam name="T">O tipo da Lista</typeparam>
        /// <param name="input">A lista a ser avaliada</param>
        public static T GetRandomItem<T>(this IList<T> input)
        {
            if (input != null)
            {
                if (input.Count == 1)
                    return input[0];
                System.Random rand = new System.Random();
                int n = rand.Next(input.Count() + 1);

                return input[n];
            }
            return (T)input;
        }

        public static List<TSource> ExceptWithDuplicates<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            List<TSource> s1 = second.ToList();
            List<TSource> ret = new List<TSource>();

            first.ToList().ForEach(n =>
            {
                if (s1.Contains(n))
                    s1.Remove(n);
                else
                    ret.Add(n);

            });

            return ret;
        }

        public static IOrderedEnumerable<T> Order<T, TKey>(this IEnumerable<T> source, System.Func<T, TKey> selector, bool ascending)
        {
            if (ascending)
            {
                return source.OrderBy(selector);
            }
            else
            {
                return source.OrderByDescending(selector);
            }
        }

        public static IOrderedEnumerable<T> ThenBy<T, TKey>(this IOrderedEnumerable<T> source, System.Func<T, TKey> selector, bool ascending)
        {
            if (ascending)
            {
                return source.ThenBy(selector);
            }
            else
            {
                return source.ThenByDescending(selector);
            }
        }
    }
}