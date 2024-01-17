using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Legends.Engine;

public class SceneObject : Spatial<SceneObject>, IDisposable, IUpdate, INamedObject, IInitalizable
{   
    private static int              _globalObjId;
    private IList<string>           _tags;    
    private IList<Ref<IBehavior>>   _behaviors;
    private IList<Ref<SceneObject>> _children;
    private IList<Ref<IComponent>>  _components;
    private Scene _scene;
    private bool _enabled = true;
    private bool _visible = true;

    public string Name { get; protected set; }  
    [DefaultValue(true)] public bool Enabled { get => _enabled; set => _enabled = value; }
    [DefaultValue(true)] public bool Visible { get => _visible; set => _visible = value; }

    [JsonIgnore] public IServiceProvider Services { get; protected set; }
    [JsonIgnore] public Scene Scene => _scene ??= GetParentScene();
    [JsonIgnore] public IEnumerable<SceneObject> Children  => ChildrenReferences.Select(n => n.Get());
    [JsonIgnore] public IEnumerable<IBehavior> Behaviors   => BehaviorReferences.Select(n => n.Get());
    [JsonIgnore] public IEnumerable<IComponent> Components => ComponentReferences.Select(n => n.Get());
    
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

    public SceneObject(IServiceProvider systems, SceneObject parent = default) : base(parent)
    {
        Services    = systems;
        Name        = string.Format("{0}#{1}", typeof(SceneObject).Name, _globalObjId++);
        
        _children   = new List<Ref<SceneObject>>();
        _behaviors  = new List<Ref<IBehavior>>();
        _components = new List<Ref<IComponent>>();
        _tags       = new List<string>();

        OriginNormalized = new Vector2(.5f, .5f);
    }

    public virtual void Initialize()
    {
        foreach(var component in Components) {
            component.Initialize();
        }  

        foreach(var behavior in Behaviors) {
            behavior.Initialize();
        }

        foreach(var child in Children) {
            child.Initialize();
        }

        EnsureBounds();
    }

    public TType GetBehavior<TType>()
        where TType: IBehavior
    {
        return Behaviors.OfType<TType>().SingleOrDefault();
    } 

    public TType GetComponent<TType>()
        where TType: class
    {
        return Components.OfType<TType>().SingleOrDefault();
    } 

    public virtual IEnumerable<SceneObject> GetObjectsAt(Vector2 position)
    {
        if(Contains(position.ToPoint()))
        {
            yield return this;

            foreach(var child in Children.SelectMany(n => n.GetObjectsAt(position)))
            {
                yield return child;
            }
        }
    }

    protected Scene GetParentScene() 
    {
        return (this is Scene scene) ? scene : Parent?.GetParentScene();
    } 

    public virtual void Update(GameTime gameTime)
    {
        if(!Enabled) return;
        
        foreach(var behavior in Behaviors) {
            behavior.Update(gameTime);
        }

        foreach(var child in Children) {
            child.Update(gameTime);
        }

        foreach(var component in Components) {
            component.Update(gameTime);
        }   
    }

    public virtual void Draw(GameTime gameTime)
    {
        if(!Visible) return;

        foreach(var behavior in Behaviors) {
            behavior.Draw(gameTime);
        }

        foreach(var component in Components) {
            component.Draw(gameTime);
        }
        
        foreach(var child in Children) {
            child.Draw(gameTime);
        }
    }

    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);

        Enabled = false;
        Visible = false;

        foreach(var behavior in Behaviors) {
            behavior.Dispose();
        }

        foreach(var child in Children) {
            child.Dispose();
        }

        foreach(var component in Components) {
            component.Dispose();
        }

        _tags.Clear();   
        _behaviors.Clear(); 
        _children.Clear(); 
        _components.Clear(); 
    }

    public void Reset()
    {
        Dispose();
        Initialize();
    }

    public override string ToString()
    {
        return  $"{Name} Pos:{Position} S:{Scale} Rot:{Rotation} E:{Enabled} Vis:{Visible}";
    }


    protected void EnsureBounds()
    {
        foreach(var child in Children) {
            EnsureBounds(child);
        }
    }

    protected void EnsureBounds(Spatial child)
    {
        if(!IsDirty) return;

        var absTopLeft          = new Vector2(Math.Min(AbsoluteTopLeft.X, child.AbsoluteTopLeft.X), Math.Min(AbsoluteTopLeft.Y, child.AbsoluteTopLeft.Y));
        var absBottomRight      = new Vector2(Math.Max(AbsoluteBottomRight.X, child.AbsoluteBottomRight.X), Math.Max(AbsoluteBottomRight.Y, child.AbsoluteBottomRight.Y));
        
        var origin              = Origin;
        
        Size                    = (absBottomRight - absTopLeft) / AbsoluteScale;
        Origin                  = origin; // adjustd the size, reset the origin back to the origional value
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