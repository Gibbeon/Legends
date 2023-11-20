using System;
using Legends.Content.Pipline;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using MonoGame.Extended.Screens;
using SlyEngine.Graphics2D;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework.Audio;
using System.Linq;

namespace Legends.App;

public class MainGame : Microsoft.Xna.Framework.Game
{
    /*
    void Do()
    {
        var _sprite = new Sprite();

        _sprite.Animator.Play(1, "sparkle_shower").Loop();

        if(true) // input left
        {
            _sprite.Move(new Vector2(-1, 0));
            _sprite.Animator.Play(0, "walk_left").Loop();
        }

        if(true) // input running left
        {
            _sprite.Move(new Vector2(-2,0));
            _sprite.Animator.Play(0, "run_left").AtSpeed(2).Loop();
        }

        if(true) // else is idling
        {
            _sprite.Animator.Play(0, "idle_left").Loop();
        }
    }
    */

    void OnMessage(object sender, EventArgs args)
    {
        switch(sender.ToString())
        {
            case "footstep":
                //AudioEmitter play sound footstep
                //PartialEmitter make footprint (check orientation)
                break;
            case "sparkle_shower":
                //ParticleEmitter play a particle shower
                break;
        }
        
    }
    private GraphicsDeviceManager _graphics;
    private ScreenManager _screenManager;
    
    public MainGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        _screenManager = new ScreenManager();
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();   

        Components.Add(_screenManager);        
    }

    protected override void LoadContent()
    {
        _screenManager.LoadScreen(new Screens.TitleScreen(this));

        Global.Fonts.LoadContent(this.Content);
        Global.Defaults.LoadContent(this.Content);

        //var value = Content.Load<SpriteData>("npc1");
        //Console.WriteLine(value.Spatial.Position);
    }

    protected override void Update(GameTime gameTime)
    {
        _screenManager.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _screenManager.Draw(gameTime);

        base.Draw(gameTime);
    }
}

