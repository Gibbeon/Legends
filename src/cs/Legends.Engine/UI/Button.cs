using System;
using System.Collections.Generic;
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

public class MultiStateComponent<TState> : Component, IRenderable, IBounds
    where TState: struct, Enum
{
    [JsonIgnore] public int         RenderLayerID => 1;
    [JsonIgnore] public bool        Visible => Parent.Visible && CurrentDrawable != null;
    [JsonIgnore] public RenderState RenderState => CurrentDrawable?.RenderState;
    [JsonIgnore] public IViewState  ViewState => Parent.Scene.Camera;
    // Indexing States directly threw KeyNotFoundException for any state the asset did not author.
    [JsonIgnore] public Drawable    CurrentDrawable => States != null && States.TryGetValue(CurrentState, out var drawable) ? drawable : null;
    public TState                   CurrentState              { get; set; }
    public Dictionary<TState, Drawable>    States      { get; set; }

    public RectangleF BoundingRectangle => CurrentDrawable?.BoundingRectangle ?? RectangleF.Empty;

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
        return BoundingRectangle;
    }

    public override void Initialize()
    {
        var states = Enum.GetValues<TState>();

        // Nothing to fall back to if the authoring data never supplied the first state.
        if(States == null || states.Length == 0 || !States.TryGetValue(states[0], out var defaultDrawable))
        {
            return;
        }

        // The final member is a count sentinel (ButtonState.Max) and intentionally gets no drawable.
        for(var i = 0; i < states.Length - 1; i++)
        {
            if(!States.ContainsKey(states[i]))
            {
                States.Add(states[i], defaultDrawable);
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
        CurrentDrawable?.DrawTo(target, Parent.Position, Parent.Rotation);
    }

    public override void Dispose()
    {
        // SceneObject.Dispose() walks its components, so throwing here took down every scene teardown.
        GC.SuppressFinalize(this);
        States?.Clear();
    }

    public bool Contains(Vector2 point)
    {
        var bounds = BoundingRectangle;
        return point.X >= bounds.Left && point.X <= bounds.Right
            && point.Y >= bounds.Top  && point.Y <= bounds.Bottom;
    }
}