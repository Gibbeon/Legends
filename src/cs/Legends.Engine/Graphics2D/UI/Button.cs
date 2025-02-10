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

public class Button : BaseUIComponent, IRenderable
{
    [JsonIgnore] public int             RenderLayerID       => 1;
    [JsonIgnore] public bool            Visible             => Parent.Visible;
    [JsonIgnore] public IViewState      ViewState           => Parent.Scene.Camera;
    [JsonIgnore] public RenderState     RenderState         => States[ButtonState].RenderState;

    public ButtonState                          ButtonState     { get; set; }
    public Dictionary<ButtonState, Drawable>    States     { get; set; }

    public Button() : this(null, null)
    {
        
    }
    public Button(IServiceProvider services, SceneObject sceneObject) 
        : base(services, sceneObject)
    {
        States = new ();
    }

    public override RectangleF GetBoundingRectangle()
    {
        return States[ButtonState].BoundingRectangle;
    }

    public override void Initialize()
    {
        for(var i = 0; i < (int)ButtonState.Max - 1; i++)
        {
            if(!States.ContainsKey((ButtonState)i))
            {
                States.Add((ButtonState)i, States[ButtonState.Default]);
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
        States[ButtonState].DrawTo(target, BoundingRectangle.TopLeft, Parent.Rotation);
    }
}