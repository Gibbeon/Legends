using Microsoft.Xna.Framework;

namespace Legends.Engine;

public class Scene : SceneObject
{
    public Camera Camera { get; set; }

    public Scene(): this(null)
    {

    }

    public Scene(SystemServices? services) : this (services, string.Empty, null)
    {

    }

    public Scene(SystemServices? services, string name, SceneObject? parent) : base(services, name, parent)
    {
        SetCamera(new Camera(services, string.Empty, this));
    }

    public void SetCamera(Camera camera)
    {
        Camera?.Dispose();
        Camera = camera;
        AttachChild(camera);
    }

    public override void Draw(GameTime gameTime)
    {        
        base.Draw(gameTime);
        Camera?.Draw(gameTime);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        Camera?.Update(gameTime);
    }
}
