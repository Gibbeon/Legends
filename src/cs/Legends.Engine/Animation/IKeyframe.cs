using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tweening;
using MonoGame.Extended;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace Legends.Engine.Animation;

public interface IKeyframe
{
    public float    Duration    { get; }
}