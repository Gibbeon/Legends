using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Legends.Engine;

public class SceneObject : Spatial, IDisposable, IUpdate, INamedObject, INotifyReload
{   
    private static int              _globalObjId;
    private IList<string>           _tags;    
    private IList<Ref<IBehavior>>   _behaviors;
    private IList<Ref<SceneObject>> _children;
    private IList<Ref<IComponent>>  _components;

    [JsonIgnore]
    public IServiceProvider Services { get; protected set; }
    
    [JsonIgnore]
    public SceneObject Parent { get; protected set; }
    
    public string Name { get; protected set; }
    
    [DefaultValue(true)]
    public bool Enabled { get; set; }
    
    [DefaultValue(true)]
    public bool Visible { get; set; }
    
    [JsonIgnore]
    public IEnumerable<SceneObject> Children
    {
        get => ChildrenReferences.Select(n => n.Get());
    }

    [JsonIgnore]
    public IEnumerable<IBehavior> Behaviors
    {
        get => BehaviorReferences.Select(n => n.Get());
    }

    [JsonIgnore]
    public IEnumerable<IComponent> Components
    {
        get => ComponentReferences.Select(n => n.Get());
    }

    [JsonProperty(nameof(Children))]
    protected IList<Ref<SceneObject>>  ChildrenReferences
    {
        get => _children;
        set => _children = value;
    }

    [JsonProperty(nameof(Behaviors))]
    protected IList<Ref<IBehavior>>  BehaviorReferences
    {
        get => _behaviors;
        set => _behaviors = value;
    }

    [JsonProperty(nameof(Components))]
    protected IList<Ref<IComponent>>  ComponentReferences
    {
        get => _components;
        set => _components = value;
    } 

    public IList<string> Tags { 
        get => _tags; 
        protected set => _tags = value; 
    }

    public SceneObject() : this(null, null)
    {

    }

    public SceneObject(IServiceProvider systems, SceneObject parent = default) : base()
    {
        Services    = systems;
        Name        = string.Format("{0}#{1}", typeof(SceneObject).Name, _globalObjId++);
        
        _children   = new List<Ref<SceneObject>>();
        _behaviors  = new List<Ref<IBehavior>>();
        _components = new List<Ref<IComponent>>();
        _tags       = new List<string>();

        Parent = parent;

        Enabled = true;
        Visible = true;
    }

    public TType GetBehavior<TType>()
        where TType: class
    {
        return Behaviors.OfType<TType>().Single();
    } 

    public TType GetComponent<TType>()
        where TType: class
    {
        return Components.OfType<TType>().SingleOrDefault();
    } 

    public override void SetPosition(Vector2 position)
    {
        base.SetPosition(position);

        if(_children != null)
        {
            foreach(var child in Children)
            {
                (child).OffsetPosition = position;
            }
        }   
    }

    public override void SetScale(Vector2 scale)
    {
        base.SetScale(scale);

        if(_children != null)
        {
            foreach(var child in Children)
            {
                (child).OffsetScale = scale;
            }
        }   
    }

    public override void SetRotation(float radians)
    {
        base.SetRotation(radians);

        if(_children != null)
        {
            foreach(var child in Children)
            {
                (child).OffsetRotation = radians;
            }
        }   
    }

    internal void AttachTo(SceneObject parent)
    {
        if(parent != null && Services == null)
        {
            Services = parent.Services;
        }

        if((Parent) != parent)
        {
            Detach();
            Parent = parent;
            parent?.AttachChild(this);
        }
    }

    internal void Detach(bool detachChildren = true)
    {
        if(detachChildren && Parent != null)
        {
            (Parent).DetachChild(this);
        }
        Parent = null;
    }

    public void AttachBehavior(IBehavior behavior)
    {
        if(!Behaviors.Contains(behavior))
        {
            _behaviors.Add((Ref<IBehavior>)behavior);
        }
    }

    public void DetachBehavior<TType>(TType behavior)
        where TType : IBehavior
    {
        if(behavior != null)
        {
            _behaviors.Remove(behavior);
        }
    }

    public void AttachComponent<TType>(TType component)
        where TType : IComponent
    {
        if(!Components.Contains(component))
        {
            _components.Add(component);
        }
    }

    public void DetachComponent<TType>()
        where TType : IComponent
    {
        DetachComponent(_components.OfType<TType>().SingleOrDefault());
    }

    protected void DetachComponent<TType>(TType component)
        where TType : IComponent
    {
        if(component != null)
        {
            _components.Remove(component);
        }
    }

    public void AttachChild(SceneObject node, bool relative = false)
    {
        if(!Children.Contains(node))
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

    public Scene GetParentScene() 
    {
        return (this is Scene scene) ? scene : (Parent).GetParentScene();
    } 

    public virtual void Update(GameTime gameTime)
    {
        if(!Enabled) return;
        
        foreach(var behavior in Behaviors)
        {
            (behavior).Update(gameTime);
        }

        foreach(var component in Components)
        {
            (component).Update(gameTime);
        }
        
        foreach(var child in Children)
        {
            (child).Update(gameTime);
        }
    }

    public virtual void Draw(GameTime gameTime)
    {
        if(!Visible) return;

        foreach(var behavior in Behaviors)
        {
            (behavior).Draw(gameTime);
        }

        foreach(var component in Components)
        {
            (component).Draw(gameTime);
        }
        
        foreach(var child in Children)
        {
            (child).Draw(gameTime);
        }
    }

    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);

        Enabled = false;
        Visible = false;

        foreach(var behavior in Behaviors)
        {
            behavior.Dispose();
        }

        foreach(var child in Children)
        {
            (child).Dispose();
        }
    }

    public void OnReload()
    {
        Dispose();
    }
}  

/*
SceneObject obj = Scene.GetObjectByName("Fency the Fence");

obj.SetState("facing", FacingDirection.Up);
obj.SetState("action", "walk");
obj.SetState("ghosted", true);
obj.SetState("imp", true);

obj.SetAnimation("walk_up", true);

controller: [
    {
        "name": "ghosted_imp_walk_up",
        "conditions": {
            "ghosted": true,
            "imp": true,
            "action": "walk",
            "facing": "Up"
        },
        "animations": [
            "walk_up", 
            "ghosted_imp", 
            "move_left", 
            "fade_in_out"
        ]
    }
]

animations: [
    {
        "$type": "KeyframeAnimation",
        "name": "walk_up"
    },
    {
        "$type": "SpriteSwapAnimation",
        "name": "ghosted_imp"
    },
    {
        "$type": "SRTAnimation",
        "name": "move_left"
    },
    {
        "$type": "ColorKeyAnimation",
        "name": "fade_in_out"
    }
]

void OnStateChanged()
{
    if(Controllers.TryFindMatch(out string[] animations))
    {
        // disable old
        foreach(var anim in Aniumations.Where(n => n.Enabled 
            && !animations.Any(n.Name)))
        {
            SetAnimation(anim, false);
        }

        // enable new
        foreach(var anim in Aniumations.Where(n => !n.Enabled 
            && animations.Any(n.Name)))
        {
            SetAnimation(anim, true);
        }
    }
}*/