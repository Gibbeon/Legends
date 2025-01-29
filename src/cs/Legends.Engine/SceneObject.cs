using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;

namespace Legends.Engine;


public class SceneObject : Spatial<SceneObject>, IAsset, IDisposable, IUpdate, IInitalizable
{   
    private IList<string>      _tags;    
    private IList<IBehavior>   _behaviors;
    private IList<SceneObject> _children;
    private IList<IComponent>  _components;
    private Scene _scene;
    private bool _enabled = true;
    private bool _visible = true;
    private string _name;

    [DefaultValue("")] public string Name { get => _name; set => _name = value; }
    [DefaultValue(true)] public bool Enabled { get => _enabled; set => _enabled = value; }
    [DefaultValue(true)] public bool Visible { get => _visible; set => _visible = value; }

    [JsonIgnore] public IServiceProvider Services { get; protected set; }
    [JsonIgnore] public Scene Scene => _scene ??= GetParentScene();

    public IBounds Bounds { get; set; }
    
    public IReadOnlyList<SceneObject> Children
    {
        get => _children.AsReadOnly();
        protected set => _children = value.ToList();
    }
    public IReadOnlyList<IBehavior> Behaviors
    {
        get => _behaviors.AsReadOnly();
        protected set => _behaviors = value.ToList();
    }
    public IReadOnlyList<IComponent>  Components
    {
        get => _components.AsReadOnly();
        protected set => _components = value.ToList();
    } 

    public IList<string> Tags { 
        get => _tags; 
        protected set => _tags = value; 
    }

    public string AssetName { get; protected set; }

    public AssetType AssetType { get; protected set; }

    public SceneObject() : this(null, null)
    {

    }

    public SceneObject(IServiceProvider systems, SceneObject parent = default) : base(parent)
    {
        Services    = systems;
        
        _children   = new List<SceneObject>();
        _behaviors  = new List<IBehavior>();
        _components = new List<IComponent>();
        _tags       = new List<string>();
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

        UpdateMatricies();
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

    public void AttachChild(SceneObject child)
    {
        _children.Add(child);
        child.SetParent(this);
    }

    /*public virtual IEnumerable<SceneObject> GetObjectsAt(Vector2 position)
    {
        if(Contains(position))
        {
            yield return this;

            foreach(var child in Children.SelectMany(n => n.GetObjectsAt(position)))
            {
                yield return child;
            }
        }
    }*/

    public IEnumerable<SceneObject> GetObjectByName(string name)
    {
        if(Equals(name, Name)) yield return this;

        foreach(var child in Children.SelectMany(n => n.GetObjectByName(name)))
        {
            yield return child;
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

    //protected override void UpdateMatricies()
    //{
    //    //foreach(var child in Children) {
        //    EnsureBounds(child);
        //}

    //    base.UpdateMatricies();
    //}

    /*protected void EnsureBounds(Spatial child)
    {
        if(!IsDirty) return;

        var absTopLeft          = new Vector2(Math.Min(AbsoluteTopLeft.X, child.AbsoluteTopLeft.X), Math.Min(AbsoluteTopLeft.Y, child.AbsoluteTopLeft.Y));
        var absBottomRight      = new Vector2(Math.Max(AbsoluteBottomRight.X, child.AbsoluteBottomRight.X), Math.Max(AbsoluteBottomRight.Y, child.AbsoluteBottomRight.Y));
        
        Size                    = (absBottomRight - absTopLeft) / AbsoluteScale;
        Position                = AbsoluteOrigin + absTopLeft / AbsoluteScale - (Parent?.AbsolutePosition ?? Vector2.Zero);
        //Origin                  = origin - (absTopLeft / AbsoluteScale); // adjustd the size, reset the origin back to the origional value
    }
    */
}  