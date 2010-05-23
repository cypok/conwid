using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Conwid.Core
{
    public static class ListExtensions
    {
        #region IEnumerable enhancements
        
        public static bool IsEmpty<T>(this IEnumerable<T> collection)
        {
            return collection.Count() == 0;
        }

        public delegate void InPlaceAction<T>(ref T obj);
        /// <summary>
        /// Returns new IEnumerable by taking a copy of each element of collection,
        /// applying an action to the copy, and collecting copies, modified by this action.
        /// </summary>
        public static IEnumerable<T> SelectFromCopies<T>(this IEnumerable<T> collection, InPlaceAction<T> action)
            where T:struct
        {
            return collection.Select(
                x => {
                    var copy = x;
                    action(ref copy);
                    return copy;
                }
            );
        }
        
        #endregion //IEnumerable enhancements
        
        #region IList enhancements

        public static void MoveToEnding<T>(this IList<T> list, int index)
        {
            var item = list.ElementAt(index);
            list.RemoveAt(index);
            list.PushBack(item);
        }
        
        public static void MoveToBeginning<T>(this IList<T> list, int index)
        {
            var item = list.ElementAt(index);
            list.RemoveAt(index);
            list.Insert(0, item);
        }
        
        public static void PushBack<T>(this IList<T> list, T item)
        {
            list.Insert(list.Count, item);
        }
        
        public static T PopFront<T>(this IList<T> list)
        {
            var item = list[0];
            list.RemoveAt(0);
            return item;
        }
        
        #endregion //IList enhancements
    }
}
