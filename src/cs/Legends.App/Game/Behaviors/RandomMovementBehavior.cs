﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Legends.Engine;
using System;
using MonoGame.Extended;
using MonoGame.Extended.Content;
using System.Reflection;

namespace Legends.App;

public class RandomMovementBehavior : BaseBehavior
{
    private float _waitTime;
    private Vector2 _targetPosition;
    private Random _random;
    public RandomMovementBehavior(SystemServices services, SceneObject parent) : base(services, parent)
    {
        _random = new Random();
        _waitTime = 1 + _random.NextSingle(4);
    }

    public override void Update(GameTime gameTime)
    {
        if(_waitTime > 0)
        {
            _waitTime -= gameTime.GetElapsedSeconds();
        }

        if(_waitTime <= 0)
        {
            if(_targetPosition != Parent.Position)
            {
                var move = (_targetPosition - Parent.Position);
                if(MathF.Abs(move.X) + Math.Abs(move.Y) > 1)
                {
                    move.Normalize();                    
                    Parent.Move(move);
                } else {
                    Parent.Position = _targetPosition;
                }                
            }

            if(Parent.Position == _targetPosition)
            {
                _waitTime = 1 + _random.NextSingle(4);
                if(_random.Next() % 2 == 1)
                {
                    _targetPosition = new Vector2(_random.Next(-320, 320), Parent.Position.Y);
                } 
                else
                {
                    _targetPosition = new Vector2(Parent.Position.X, _random.Next(-200, 200));
                }
            }
        }
    }
}