using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace LitEngine.Framework.Collections
{   public class UpdateableOrderComparer : IComparer<IUpdateable>
    {
        public static UpdateableOrderComparer Default = new UpdateableOrderComparer();

        public int Compare(IUpdateable? a, IUpdateable? b)
        {
            if(a == null || b == null) {
                return -1;
            }

            return a.UpdateOrder.CompareTo(b.UpdateOrder);
        }
    }
}
