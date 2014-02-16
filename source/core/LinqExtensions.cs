using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Se7enRedLines
{
    public static class LinqExtensions
    {
        //======================================================
        #region _Public methods_

        public static void RemoveAll<TKey, TValue>(this Dictionary<TKey, TValue> dict,
                                                   Func<KeyValuePair<TKey, TValue>, bool> condition)
        {
            foreach (var cur in dict.Where(condition).ToList())
                dict.Remove(cur.Key);
        }

        public static void RemoveAll<T>(this ICollection<T> collection, Func<T, bool> condition)
        {
            if (collection.IsReadOnly)
                throw new InvalidOperationException("Cannot perform operation on read only collection");

            foreach (var item in collection.Where(condition).ToList())
                collection.Remove(item);
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
                action(item);
        }

        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> values)
        {
            foreach (var value in values)
                collection.Add(value);
        }

        #endregion
    }
}