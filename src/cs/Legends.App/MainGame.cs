using System;
using Legends.Content.Pipline;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using MonoGame.Extended.Screens;
using System.Collections;
using System.Collections.Generic;

namespace Legends.App;


public abstract class Ref
{
    private int _ref;
    public void AddRef() { _ref++; }
    public void RemoveRef() { _ref--; }
    public abstract Type GetRefType();
}

public class Ref<TType> : Ref
{
    private TType _value;

    public static implicit operator TType(Ref<TType> value)
    {
        return value._value;
    }

    public Ref(TType value)
    {
        _value = value;
    }

    public override Type GetRefType()
    {
    return typeof(TType);
    }
}


public class DynamicContentManager : ContentManager
{
    IList<Ref> _references;

    public DynamicContentManager(IServiceProvider serviceProvider): base(serviceProvider)
    {
        _references = new List<Ref>();
    }

    public Ref<TType> LoadRef<TType>(string name)
    { 
        var item = new Ref<TType>(base.Load<TType>(name));
        _references.Add(item);
        return item;
    }
}

public class MainGame : Game
{
    private GraphicsDeviceManager _graphics;
    private ScreenManager _screenManager;
    
    public MainGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content = new DynamicContentManager(this.Services);
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

        Global.Fonts.LoadContent(this.Content as DynamicContentManager);
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

