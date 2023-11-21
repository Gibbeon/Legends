using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace SlyEngine.Graphics2D;
public class GameObject : Spatial, IDisposable
{   
    public class GameObjectDesc : SpatialDesc
    {
        public string Name;
    }

    public SystemServices Services { get; private set; }

    public string Name { get; set; }

    private IList<string> _tags;
    public IList<string> Tags => _tags;

    private readonly IList<IBehavior> _behaviors;

    private readonly IList<GameObject> _children;

    public GameObject? Parent { get; private set; }

    public IReadOnlyList<GameObject> Children
    {
        get => _children.ToList().AsReadOnly();
    }

    public IReadOnlyList<IBehavior> Behaviors
    {
        get => _behaviors.ToList().AsReadOnly();
    }

    public GameObject(SystemServices systems) : this(systems, new GameObjectDesc())
    {

    }

    public GameObject(SystemServices systems, GameObject? parent) : this(systems, new GameObjectDesc())
    {
        if(parent != null)
        {
            this.AttachTo(parent);
        }
    }

    public GameObject(SystemServices systems, GameObjectDesc data) : base(data)
    {
        Services = systems;
        Name = data.Name;
        _children = new List<GameObject>();
        _behaviors = new List<IBehavior>();
        _tags = new List<string>();
    }

    public TType? GetBehavior<TType>()
        where TType: class
    {
        return _behaviors.SingleOrDefault(n => n.GetType() == typeof(TType)) as TType;
    } 

    public override void SetPosition(Vector2 position)
    {
        base.SetPosition(position);

        if(_children != null)
        {
            foreach(var item in _children)
            {
                item.OffsetPosition = position;
            }
        }   
    }

    public override void SetScale(Vector2 scale)
    {
        base.SetScale(scale);

        if(_children != null)
        {
            foreach(var item in _children)
            {
                item.OffsetScale = scale;
            }
        }   
    }

    public override void SetRotation(float radians)
    {
        base.SetRotation(radians);

        if(_children != null)
        {
            foreach(var item in _children)
            {
                item.OffsetRotation = radians;
            }
        }   
    }

    internal void AttachTo(GameObject parent)
    {
        if(Parent != parent)
        {
            Detach();
            Parent = parent;
            parent?.AttachChild(this);
        }
    }

    internal void Detach(bool detachChildren = true)
    {
        if(detachChildren)
        {
            Parent?.DetachChild(this);
        }
        Parent = null;
    }

    public void AttachChild(GameObject node, bool relative = false)
    {
        if(!_children.Contains(node))
        {
            _children.Add(node);
            node.AttachTo(this);

            if(relative)
            {
                node.OffsetPosition   = this.Position;
                node.OffsetRotation   = this.Rotation;
                node.OffsetScale      = this.Scale;
            }
        }
    }

    public void DetachChild(GameObject node, bool relative = false)
    {
        if(_children.Remove(node))
        {
            if(relative)
            {
                node.OffsetPosition   = this.Position;
                node.OffsetRotation   = this.Rotation;
                node.OffsetScale      = this.Scale;
            }

            node.Detach(false);
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}  