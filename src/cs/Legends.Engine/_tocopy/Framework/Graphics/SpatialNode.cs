using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using LitEngine.Framework.Collections;

namespace LitEngine.Framework.Graphics
{
    public class SpatialNode : Spatial
    {
        public Spatial? Parent
        {
            get;
            protected set;
        }
        public IReadOnlyList<Spatial> Children
        {
            get => _children;
        }
        private List<Spatial> _children;
        private UpdateableListIndex<Spatial> _childrenUpdateIndex;

        public SpatialNode() : this(null)
        {

        }

        public SpatialNode(Spatial? parent)
        {
            _children = new List<Spatial>();
            _childrenUpdateIndex = new UpdateableListIndex<Spatial>(_children);
            Parent = Parent;
        }
        public virtual void Add(Spatial animation)
        {
            _childrenUpdateIndex.Add(animation);
        }

        public virtual void Remove(Spatial animation)
        {
            _childrenUpdateIndex.Remove(animation);
        }

        internal override void UpdateSpatial()
        {
            bool updateGlobal = HasChanged;

            this.UpdateRelative(Parent);

            foreach (var child in _childrenUpdateIndex)
            {
                //Console.WriteLine("UpdateSpatial @ {0}, {1}", child.Position.X, child.Position.Y);
                child.UpdateRelative(this, updateGlobal);
            }
        }
    }
}


