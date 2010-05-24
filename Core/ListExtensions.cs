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
        public delegate void InPlaceAction<T1, T2>(ref T1 obj1, T2 obj2);
        /// <summary>
        /// Returns a new IEnumerable by taking a copy of each element of collection,
        /// applying an action to the copy, and collecting the copies, modified by this action.
        /// </summary>
        public static IEnumerable<T> SelectFromCopies<T>(this IEnumerable<T> collection, InPlaceAction<T> action)
            where T:struct
        {
            return collection.Select(
                x => {
                    action(ref x);
                    return x;
                }
            );
        }

        /// <summary>
        /// Returns a new IEnumerable by taking for each pair of items from two collections 
        /// the copy of item of first collection, applying an action to a copy and an item
        /// of second collection, and collecting the copies, modified by this action
        /// </summary>
        public static IEnumerable<T1> SelectFromPairs<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second, InPlaceAction<T1, T2> action)
            where T1:struct
        {
            foreach(var item_of_first in first)
            {
                foreach(var item_of_second in second)
                {
                    var copy = item_of_first;
                    action(ref copy, item_of_second);
                    yield return copy;
                }
            }
        }

        /// <summary>
        /// Returns the first element of collection if it isn't empty,
        /// and given object if it is
        /// </summary>
        public static T FirstOrGiven<T>(this IEnumerable<T> collection, T given)
        {
            if(collection.IsEmpty())
                return given;
            else
                return collection.First();
        }

        public static bool HasSameElements<T>(this IEnumerable<T> collection, IEnumerable<T> another)
        {
            return collection.All( x => another.Contains(x) ) && another.All( x => collection.Contains(x) );
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
