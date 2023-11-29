using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Legends.Engine;

public class SceneObject : Spatial, IDisposable, IUpdate
{   
    private static int _globalObjId;
    private IList<string> _tags;    
    private IList<IBehavior> _behaviors;
    private IList<SceneObject> _children;

    [JsonIgnore]
    public IServiceProvider? Services { get; private set; }
    
    [JsonIgnore]
    public SceneObject? Parent { get; private set; }
    
    public string Name { get; set; }
    
    public IList<string> Tags { get => _tags; protected set => _tags = value; }
    
    [DefaultValue(true)]
    public bool Enabled { get; set; }
    
    [DefaultValue(true)]
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

    public SceneObject(IServiceProvider? systems, SceneObject? parent = default) : base()
    {
        Services    = systems;
        Name        = string.Format("{0}#{1}", typeof(SceneObject).Name, _globalObjId++);
        
        _children   = new List<SceneObject>();
        _behaviors  = new List<IBehavior>();
        _tags       = new List<string>();

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

    public Scene? GetParentScene() 
    {
        return (Parent is Scene) ? (Scene)Parent : Parent?.GetParentScene();
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

    public virtual void Dispose()
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
}  