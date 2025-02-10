using System;
using System.Collections.Generic;
using Autofac.Core.Lifetime;
using Legends.Engine.Graphics2D;
using Legends.Engine.Graphics2D.Components;
using Legends.Engine.Graphics2D.Primitives;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Newtonsoft.Json;


namespace Legends.Engine.UI;

public enum ButtonState
{
    Default,
    Pressed,
    Hover,
    Max
}

public class Button: UISceneObject
{
    public Button(IServiceProvider services, SceneObject sceneObject = default) 
        : base(services, sceneObject)
    {
        
    }
}

public class MultiStateComponent<TState> : Component, IRenderable
    where TState: struct, Enum
{
    [JsonIgnore] public int         RenderLayerID => 1;
    [JsonIgnore] public bool        Visible => Parent.Visible;
    [JsonIgnore] public RenderState RenderState => CurrentDrawable.RenderState;
    [JsonIgnore] public IViewState  ViewState => Parent.Scene.Camera;
    [JsonIgnore] public Drawable    CurrentDrawable => States[CurrentState]; 
    public TState                   CurrentState              { get; set; }
    public Dictionary<TState, Drawable>    States      { get; set; }

    public MultiStateComponent() : this(null, null)
    {
        
    }
    public MultiStateComponent(IServiceProvider services, SceneObject sceneObject = default) 
        : base(services, sceneObject)
    {
        States = new ();
    }

    public RectangleF GetBoundingRectangle()
    {
        return CurrentDrawable.BoundingRectangle;
    }

    public override void Initialize()
    {
        for(var i = 0; i < (int)Enum.GetValues<TState>().Length - 1; i++)
        {
            var state = Enum.GetValues<TState>()[i];
            if(!States.ContainsKey(state))
            {
                States.Add(state, States[state]);
            }
        }
    }

    public override void Draw(GameTime gameTime)
    { 
        Services.Get<IRenderService>().DrawItem(this);
    }

    public override void Reset()
    {

    }

    public void DrawImmediate(GameTime gameTime, RenderSurface target)
    {
        CurrentDrawable.DrawTo(target, Parent.Position, Parent.Rotation);
    }

    public override void Dispose()
    {
        throw new NotImplementedException();
    }
}