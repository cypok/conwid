using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Conwid.Core
{
    public static class ListExtensions
    {
        public static bool IsEmpty<T>(this IEnumerable<T> collection)
        {
            return collection.Count() == 0;
        }
        public static void MoveToEnding<T>(this IList<T> list, int index)
        {
            var item = list.ElementAt(index);
            list.RemoveAt(index);
            list.Insert(list.Count, item);
        }
        public static void MoveToBeginning<T>(this IList<T> list, int index)
        {
            var item = list.ElementAt(index);
            list.RemoveAt(index);
            list.Insert(0, item);
        }
    }
}
