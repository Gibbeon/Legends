using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Legends.Engine;

public class SceneObject : Spatial, IDisposable, IUpdate
{   
    public class SceneObjectDesc : SpatialDesc
    {
        public string Name;
        public bool Enabled = true;
        public bool IsVisible = true;
        public IList<string> Tags;        
        public IList<SceneObjectDesc> Children;
        public IList<IBehavior.BehaviorDesc> Behaviors;

        public SceneObjectDesc()
        {
            Name        = "";
            Tags        = new List<string>();
            Children    = new List<SceneObject.SceneObjectDesc>();
            Behaviors   = new List<IBehavior.BehaviorDesc>();
        }
    }

    public Scene? ParentScene => (Parent is Scene) ? (Scene)Parent : Parent?.ParentScene;

    public SystemServices Services { get; private set; }

    public string Name { get; set; }

    private IList<string> _tags;
    public  IList<string> Tags { get => _tags; protected set => _tags = value; }
    private IList<IBehavior>       _behaviors;
    private IList<SceneObject>     _children;
    public SceneObject? Parent { get; private set; }
    public bool Enabled { get; set; }
    public bool IsVisible { get; set; }
    public IReadOnlyList<SceneObject> Children
    {
        get => _children.ToList().AsReadOnly();
        protected set => _children = (IList<SceneObject>)value;
    }

    public IReadOnlyList<IBehavior> Behaviors
    {
        get => _behaviors.ToList().AsReadOnly();
        protected set => _behaviors = (IList<IBehavior>)value;
    }

    public SceneObject(SystemServices systems) : this(systems, null, new SceneObjectDesc())
    {

    }

    public SceneObject(SystemServices systems, SceneObject? parent) : this(systems, parent, new SceneObjectDesc())
    {

    }

    public SceneObject(SystemServices systems, SceneObject? parent, SceneObjectDesc data) : base(data)
    {
        Services = systems;
        Name = data.Name;
        Enabled = true;
        IsVisible = true;
        
        _children = new List<SceneObject>();
        _behaviors = new List<IBehavior>();
        _tags = new List<string>();

        if(parent != null)
        {
            this.AttachTo(parent);
        }
    }

    public TType GetBehavior<TType>()
        where TType: class
    {
        return _behaviors.OfType<TType>().Single();
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

    internal void AttachTo(SceneObject parent)
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

    public void AttachBehavior(IBehavior behavior)
    {
        _behaviors.Add(behavior);
    }

    public void DetachBehavior<TType>()
        where TType : IBehavior
    {
        _behaviors.Remove(_behaviors.OfType<TType>().Single());
    }

    public void AttachChild(SceneObject node, bool relative = false)
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

    public void DetachChild(SceneObject node, bool relative = false)
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
        Enabled = false;
        IsVisible = false;

        foreach(var behavior in Behaviors)
        {
            behavior.Dispose();
        }
        
        foreach(var child in Children)
        {
            child.Dispose();
        }
    }

    public virtual void Draw(GameTime gameTime)
    {
        if(!IsVisible) return;

        foreach(var behavior in Behaviors)
        {
            behavior.Draw(gameTime);
        }
        
        foreach(var child in Children)
        {
            child.Draw(gameTime);
        }
    }

    public virtual void Update(GameTime gameTime)
    {
        if(!Enabled) return;
        
        foreach(var behavior in Behaviors)
        {
            behavior.Update(gameTime);
        }
        
        foreach(var child in Children)
        {
            child.Update(gameTime);
        }
    }
}  