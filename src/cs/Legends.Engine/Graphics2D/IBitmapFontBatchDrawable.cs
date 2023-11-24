using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.Sprites;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.TextureAtlases;

using System;
using System.Security;
using System.ComponentModel;
using Legends.Engine.Graphics2D;
using MonoGame.Extended.Graphics.Effects;
using Autofac.Core;

namespace Legends.Engine;

public interface IBitmapFontBatchDrawable : IBatchDrawable<BitmapFont>
{
    public string Text { get; }

}
