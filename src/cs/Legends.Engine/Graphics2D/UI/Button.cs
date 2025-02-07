using System.ComponentModel.Design;
using Legends.Engine.Graphics2D;
using Legends.Engine.Graphics2D.Components;
using Microsoft.Xna.Framework;
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
    [JsonIgnore] public int             RenderLayerID => 1;
    [JsonIgnore] public bool            Visible => Parent.Visible;
    [JsonIgnore] public IViewState      ViewState => Parent.Scene.Camera;
    [JsonIgnore] public RenderState     RenderState     => Backgrounds[(int)ButtonState].RenderState;

    public ButtonState  ButtonState     { get; set; }
    public Sprite[]     Backgrounds     { get; set; }

    public Button(IServiceContainer services, SceneObject sceneObject) 
        : base(services, sceneObject)
    {
        Backgrounds = new Sprite[(int)ButtonState.Max];
    }

    public override void Initialize()
    {
        for(var i = 0; i < (int)ButtonState.Max; i++)
        {
            Backgrounds[i] ??= Backgrounds[(int)ButtonState.Default];
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
        Backgrounds[(int)ButtonState].DrawTo(target, (Rectangle)BoundingRectangle, Parent.Rotation);
    }
}