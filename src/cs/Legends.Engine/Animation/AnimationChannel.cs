using Microsoft.Xna.Framework;

namespace Legends.Engine.Animation;

public class AnimationChannel
{
    public AnimationController Controller { get; protected set; }
    public IAnimationData AnimationData { get; protected set; }
    public object State { get; set; }    
    public bool Enabled { get; set; }    

    public AnimationChannel(AnimationController controller, IAnimationData data)
    {
        Controller = controller;
        AnimationData = data;
        if(controller.Parent != null)
        {
            Enabled = data.Enabled;
            data.InitializeChannel(this);
        }
    }

    public void Update(GameTime gameTime)
    {
        AnimationData.UpdateChannel(this, gameTime);
    }
}