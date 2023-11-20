using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using SlyEngine.Animation;

namespace SlyEngine.Graphics2D;

public class Sprite : SpatialNode
{
    public AnimationController Animation;
    //public ParticleEmitter Particles;

    public Sprite() : this(null)
    {

    }
    public Sprite(SpatialNode? parent) : base(parent)
    {
        Animation = new AnimationController();
    }
}