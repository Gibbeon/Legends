using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Legends.Engine.Graphics2D;

public class Scene : SceneObject
{
    public Legends.Engine.Graphics2D.Camera? Camera { get; set; }

    public Scene(SystemServices services) : this (services, null)
    {
        Camera = new Camera(services);
    }

    public Scene(SystemServices services, SceneObject? parent) : base(services, parent)
    {

    }

    public override void Draw(GameTime gameTime)
    {
        Services.GetService<IRenderService>().SetCamera(Camera);
        base.Draw(gameTime);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }
}

public class SceneObject : Spatial, IDisposable, IUpdate
{   
    public class SceneObjectDesc : SpatialDesc
    {
        public string Name;
    }

    public SceneObject? ParentScene => (Parent is Scene) ? Parent : Parent?.ParentScene;

    public SystemServices Services { get; private set; }

    public string Name { get; set; }

    private IList<string> _tags;
    public IList<string> Tags => _tags;

    private readonly IList<IBehavior> _behaviors;

    private readonly IList<SceneObject> _children;

    public SceneObject? Parent { get; private set; }

    public bool Enabled { get; set; }
    public bool IsVisible { get; set; }

    public IReadOnlyList<SceneObject> Children
    {
        get => _children.ToList().AsReadOnly();
    }

    public IReadOnlyList<IBehavior> Behaviors
    {
        get => _behaviors.ToList().AsReadOnly();
    }

    public SceneObject(SystemServices systems) : this(systems, new SceneObjectDesc())
    {

    }

    public SceneObject(SystemServices systems, SceneObject? parent) : this(systems, new SceneObjectDesc())
    {
        if(parent != null)
        {
            this.AttachTo(parent);
        }
    }

    public SceneObject(SystemServices systems, SceneObjectDesc data) : base(data)
    {
        Services = systems;
        Name = data.Name;
        Enabled = true;
        IsVisible = true;
        
        _children = new List<SceneObject>();
        _behaviors = new List<IBehavior>();
        _tags = new List<string>();
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