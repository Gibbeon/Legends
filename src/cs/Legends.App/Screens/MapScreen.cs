using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;
using Legends.Engine.Input;
using MonoGame.Extended.Input.InputListeners;
using Legends.Engine;
using System;
using Legends.Engine.Content;
using System.Linq;
using System.ComponentModel;
using Legends.Engine.Graphics2D.Components;

namespace Legends.App.Screens;

public class GameDateTime
{
    public int Day { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public int Hours { get; set; }
    public int Minutes { get; set; }
    private double _elapsedTime;
    private int _millisecondsPerMinute = 1000;
    private const int MINUTES_IN_HOUR = 60;
    private const int HOURS_IN_DAY = 24;
    private const int DAYS_IN_MONTH = 30;
    private const int MONTHS_IN_YEAR = 12;

    public void Initialize()
    {
        Day = 1;
        Month = 1;
        Year = 1;
        Hours = 8;
        Minutes = 0;
    }

    public void Update(GameTime gameTime)
    {
        _elapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;

        while(_elapsedTime > _millisecondsPerMinute)
        {
            _elapsedTime -=_millisecondsPerMinute;
            Increment();
        }
    }

    public string ToTimeString()
    {
        return string.Format("{0,2}:{1,2}", Hours.ToString("D2"),Minutes.ToString("D2"));
    }

    public string ToDateString()
    {
        return string.Format("Month: {0}, Day: {1}, Year: {2}", Month, Day, Year);
    }

    public void Increment()
    {
        Minutes++;

        while(Minutes >= MINUTES_IN_HOUR)
        {
            Minutes -= MINUTES_IN_HOUR;

            Hours++;

            while(Hours >= HOURS_IN_DAY)
            {
                Hours -= HOURS_IN_DAY;

                Day++;

                while(Day > DAYS_IN_MONTH)
                {
                    Day -= DAYS_IN_MONTH;

                    Month++;

                    while(Month > MONTHS_IN_YEAR)
                    {
                        Month -= MONTHS_IN_YEAR;
                        
                        Year++;
                    }
                }
            }

        }
    }
}

public class MapScreen : Screen
{
    private Ref<Scene>[] _scenes;
    private IServiceProvider _services;
    private InputManager _input;

    private SceneObject _soTime;
    private SceneObject _soDate;

    GameDateTime _gameDateTime = new();

    public MapScreen(IServiceProvider services)
    {
        _services = services;
        _scenes = new Ref<Scene>[2];
        _scenes[0] = _services.GetContentManager().GetRef<Scene>("Maps/WorldMap");
        _scenes[1] = _services.GetContentManager().GetRef<Scene>("Scenes/HUD/HudScene");

        _soTime = (~_scenes[1]).GetObjectByName("time").Single();
        _soDate = (~_scenes[1]).GetObjectByName("date").Single();
    }

    public override void Initialize()
    {
        base.Initialize();

        _input = new InputManager(_services, new KeyboardListenerSettings()
        {
            InitialDelayMilliseconds = 0,
            RepeatDelayMilliseconds = 0,
            RepeatPress = true
        });

        (~_scenes[0]).Initialize();
        (~_scenes[1]).Initialize();

        _gameDateTime.Initialize();
    }

    public override void Draw(GameTime gameTime)
    {        
        (~_scenes[0]).Draw(gameTime);
        (~_scenes[1]).Draw(gameTime);
    }

    public override void Update(GameTime gameTime)
    {   
        _gameDateTime.Update(gameTime);

        _soTime.GetComponent<TextLabel>().Text = _gameDateTime.ToTimeString();
        _soDate.GetComponent<TextLabel>().Text = _gameDateTime.ToDateString();

        (~_scenes[0]).Update(gameTime);
        (~_scenes[1]).Update(gameTime);
    }
}