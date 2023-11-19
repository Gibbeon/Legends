using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlyEngine.Graphics.Collections;

namespace SlyEngine.Graphics;
    /*
    public class Canvas2D
    {
        public Game Game { get; private set; }
        public Camera2D Camera
        {
            get;
            private set;
        }
        public IReadOnlyList<Layer2D> Layers
        {
            get => _layers;
        }
        private List<Layer2D> _layers;
        protected UpdateableListIndex<Layer2D> _updateableListIndex;
        protected DrawableListIndex<Layer2D> _drawableListIndex;

        public Canvas2D(Game game)
        {
            Game = game;
            Camera = new Camera2D(Game.GraphicsDevice);
            _layers = new List<Layer2D>();
            _updateableListIndex = new UpdateableListIndex<Layer2D>(_layers);
            _drawableListIndex = new DrawableListIndex<Layer2D>(_layers);
        }
        public void Update(GameTime gameTime)
        {
            foreach (var item in _updateableListIndex)
            {
                item.Update(gameTime);
            }

            _drawableListIndex.Update(); // resorts if needed based on draw order changes
        }

        public void Draw(GameTime gameTime)
        {
            foreach (var item in _drawableListIndex)
            {
                item.Draw(gameTime);
            }
        }

        public Layer2D Add()
        {
            var item = new Layer2D(this);
            _updateableListIndex.Add(item);
            _drawableListIndex.Add(item);
            return item;
        }

        public void Remove(Layer2D item)
        {
            var index = _layers.IndexOf(item);
            _updateableListIndex.RemoveIndexOnlyAt(index);
            _drawableListIndex.RemoveAt(index);
        }
    }
    */
