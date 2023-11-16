// using System;
// using System.Collections.Generic;
// using Microsoft.Xna.Framework;

// namespace Legends.App.Collections
// {
//     public static class ListExtensions {
//         public static void RemoveAll<TType>(this IList<TType> list, Func<TType, bool> func)
//         {
//             var current = list.Count - 1;

//             while(current > -1) {
//                 if(func(list[current])) {
//                     list.RemoveAt(current);
//                 }
//             }
//         }
//     }
// }
