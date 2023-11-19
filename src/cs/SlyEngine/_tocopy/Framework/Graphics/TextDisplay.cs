using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace LitEngine.Framework.Graphics
{
    public enum VerticalTextAlign
    {
        None,
        Top,
        Middle,
        Bottom
    }

    public enum HorizontalTextAlign
    {
        None,
        Left,
        Center,
        Right
    }

    class TextDisplay : Sprite2D
    {
        public SpriteFont Font
        {
            get;
            set;
        }
        public VerticalTextAlign VerticalTextAlign
        {
            get;
            set;
        }
        public HorizontalTextAlign HorizontalTextAlign
        {
            get;
            set;
        }
        public string Text
        {
            get => _text;
            set => SetText(value);
        }
        private string _text = string.Empty;
        public TextDisplay(string text, SpriteFont font, VerticalTextAlign verticalTextAlign, HorizontalTextAlign horizontalTextAlign) : base(0, 0, 0, 0, null)
        {
            Font = font;

            HorizontalTextAlign = horizontalTextAlign;
            VerticalTextAlign = verticalTextAlign;

            SetText(text);
        }

        public void SetText(string text)
        {
            _text = text;
            var size = Font.MeasureString(_text);

            Width = size.X;
            Height = size.Y;

            NeedToUpdate();
        }

        public override void DrawBatched(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if(!Visible) return;

            var yOffset = 0;
            var xOffset = 0;

            switch (VerticalTextAlign)
            {
                case VerticalTextAlign.None:
                case VerticalTextAlign.Top:
                    break;
                case VerticalTextAlign.Middle:
                    yOffset = (int)(((GameEngine.Instance.GraphicsDevice.Viewport.Height - GameEngine.Instance.GraphicsDevice.Viewport.Y) / 2) - Height / 2);
                    break;
                case VerticalTextAlign.Bottom:
                    yOffset = (int)((GameEngine.Instance.GraphicsDevice.Viewport.Height - GameEngine.Instance.GraphicsDevice.Viewport.Y) - Height);
                    break;
            }

            switch (HorizontalTextAlign)
            {
                case HorizontalTextAlign.None:
                case HorizontalTextAlign.Left:
                    break;
                case HorizontalTextAlign.Center:
                    xOffset = (int)(((GameEngine.Instance.GraphicsDevice.Viewport.Width - GameEngine.Instance.GraphicsDevice.Viewport.X) / 2) - Width / 2);
                    break;
                case HorizontalTextAlign.Right:
                    xOffset = (int)((GameEngine.Instance.GraphicsDevice.Viewport.Width - GameEngine.Instance.GraphicsDevice.Viewport.X) - Width);
                    break;
            }

            spriteBatch.DrawString(Font, _text, new Vector2(Position.X + xOffset, Position.Y + yOffset), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects, Depth);
        }
    }
}
