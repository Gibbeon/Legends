using System;
using Microsoft.Xna.Framework;

namespace SlyEngine.Animation;

public class AnimationChannel
{
    public IAnimation? Current { get; set; }
    public float Speed { get; set; }
    public bool Enabled { get; set; }
    public AnimationChannel Loop(LoopType type = LoopType.Loop) 
    {   
        if(Current != null)
        {
            Current.LoopType = type; 
        }
        return this; 
    }

    public AnimationChannel AtSpeed(float value) 
    { 
        Speed = value; 
        return this; 
    }

    public AnimationChannel Play(IAnimation? animation, EventHandler<AnimationMessageCallbackEventArgs>? callback = default)
    {
        if(Current == animation || Current?.Name == animation?.Name) return this;
        
        if(Current != null)
        {
            Current.MessageCallback -= callback;
        }
        
        Current = animation?.Clone();

        if(Current != null)
        {   
            Current.MessageCallback += callback;
        }

        return this;
    }

    public AnimationChannel Pause()
    {
        Enabled = false;
        return this;
    }

    public AnimationChannel Resume()
    {
        Enabled = true;
        return this;
    }

    public void Update(GameTime gameTime)
    {   
        if(!Enabled) return;
        Current?.Update(new GameTime(gameTime.TotalGameTime, gameTime.ElapsedGameTime * Speed));
    }
}
