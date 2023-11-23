using System;
using System.IO;
using LitEngine.Framework;
using LitEngine.Framework.Graphics;
using LitEngine.Framework.Graphics.Animation;
using LitEngine.Framework.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus;

namespace LitEngine.Framework.Graphics.Animation
{
    public struct FrameSet
    {
        public Rectangle Source { get; private set; }
        public int FrameWidth { get; private set; }
        public int FrameHeight { get; private set; }

        public FrameSet(Rectangle source, int frameWidth, int frameHeight)
        {
            Source = source;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
        }

        public Rectangle GetFrame(int frame)
        {
            return new Rectangle(Source.X + FrameWidth * frame, FrameWidth * frame, FrameWidth, FrameHeight);
        }
    }


}