using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LitEngine.Framework.States;
using LitEngine.Framework.Graphics.Animation;

namespace LitEngine.Framework
{

    // public static class GlobalDevices 
    // {
    //     public static GraphicsDeviceManager _DeviceGraphics;
    //     public static SpriteBatch           _Device2D;  
    //     public static GraphicsDevice        _Device3D;
    //     public static BasicEffect           _DeviceBasicFX;    
    //     public static ContentManger         _DeviceContent;

    //     do initialization 
    //     do disposing
    // }

    public class GameEngine : Microsoft.Xna.Framework.Game
    {
        public static GameEngine Instance { get; protected set; }
        public GraphicsDeviceManager GraphicsDeviceManager { get; protected set; }
        public IGameStateMachine GameStateMachine { get; protected set; }
        public AnimationController AnimationController { get; protected set; }
        public bool Clear { get; set; }
        public Color ClearColor { get; set; }
        public GameEngine()
        {
            Instance = this;

            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            GraphicsDeviceManager.SynchronizeWithVerticalRetrace = false;

            IsFixedTimeStep = false;
            Content.RootDirectory = "Content";

            Clear = true;
            ClearColor = Color.Black;
        }

        protected override void Initialize()
        {
            base.Initialize();

            AnimationController = new AnimationController();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(AnimationController != null) AnimationController.Update(gameTime);
            if(GameStateMachine != null) GameStateMachine.Update(gameTime);
            
        }

        protected override void Draw(GameTime gameTime)
        {            
            base.Draw(gameTime);

            if(Clear) GraphicsDevice.Clear(ClearColor);

            if(GameStateMachine != null) GameStateMachine.Draw(gameTime);   
        }

        public void SetState(IGameState state) {
            GameStateMachine.SetState(state);
        }
    }

}