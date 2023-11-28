using System;
using System.Linq;
using System.Collections.Generic;
using Legends.Engine.Graphics2D;
using Microsoft.Xna.Framework;
using Microsoft.VisualBasic;

namespace Legends.Engine.Animation;

public class AnimationBehavior : BaseBehavior
{
    public IList<IAnimation> Animations { get; private set; }
    public IAnimation? Current { get; set; }
    public float Speed { get; set; }
    public bool Enabled { get; set; }

    public EventHandler<AnimationMessageCallbackEventArgs>? MessageCallback;
    
    public AnimationBehavior(SystemServices? services, SceneObject? parent) : base(services, parent)
    {
        Animations = new List<IAnimation>();
        Speed = 1;
    }
    public void Play(string name) 
    {
        if(Current == null || Current.Name != name)
        {
            Current = Animations.Single(n => n.Name == name);
            Current?.Initialize();
        }
    }

    public override void Update(GameTime gameTime)
    {
        Current?.Update(new GameTime(gameTime.TotalGameTime, gameTime.ElapsedGameTime.Multiply(Speed)));
    }

    public override void Dispose()
    {
        
    }
}

