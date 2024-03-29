﻿using MonoGame.Extended;
using System.Linq;
using System;
using Legends.Engine.Animation;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Buffers;

namespace Legends.Engine.Graphics2D.Components;

public class TileSet
{
    [JsonIgnore]
    public Size2            TileSize => TextureRegion.Slice;

    [JsonIgnore]
    public TextureRegion    TextureRegion => TextureRegionReference.Get();
 
    [JsonProperty(nameof(TextureRegion))]
    protected Ref<TextureRegion> TextureRegionReference { get; set; }
 
    public Dictionary<string, ushort[]> Tags { get; set; }

    private uint _stride;
    private float _uvwidth;
    private float _uvheight;
    private uint _localstride;
    private uint _xofs;
    private uint _yofs;
    private ushort[][] _tagIndex;
    private string[] _tags;

    private ushort[] _index;

    public TileSet()
    {
        Tags = new();
    }

    public void Initialize()
    {
        TextureRegion.Initialize();
        
        _stride     = (uint)(TextureRegion.Texture.Width / TileSize.Width);
        _uvwidth    = TileSize.Width /  TextureRegion.Texture.Width;
        _uvheight   = TileSize.Height / TextureRegion.Texture.Height; 

        _localstride = (uint)(TextureRegion.Size.Width / TileSize.Width);

        _xofs = (uint)(TextureRegion.Position.X / TileSize.Width);
        _yofs = (uint)(TextureRegion.Position.Y / TileSize.Height);

        _tags = Tags.Keys.ToArray();
        _tagIndex = new ushort[(int)TextureRegion.TileCount.Height * (int)TextureRegion.TileCount.Width][];
        ushort tempIndex = 0;
        _index = Enumerable.Repeat(tempIndex++, (int)TextureRegion.TileCount.Height * (int)TextureRegion.TileCount.Width).ToArray();

        for(ushort tagIndex = 0; tagIndex < _tags.Length; tagIndex++)
        {
            var indicies = Tags[_tags[tagIndex]];
            foreach(var index in indicies)
            {
                if(_tagIndex[index] == null) _tagIndex[index] = new ushort[0];
                
                var array = new ushort[_tagIndex[index].Length + 1];
                _tagIndex[index].CopyTo(array, 0);
                array[array.Length - 1] = tagIndex;
                _tagIndex[index] = array;
            }            
        }
    }

    public IEnumerable<string> GetTags(ushort tileIndex)
    {
        foreach(var index in _tagIndex[tileIndex])
        {
            yield return _tags[index];
        }
    }

    public IEnumerable<ushort> GetByTag(string tag)
    {
        return Tags.TryGetValue(tag, out ushort[] array) ? array : Enumerable.Empty<ushort>();
    }

    public void AddTag(string tag, ushort tileIndex)
    {
        if(!Tags.TryGetValue(tag, out ushort[] array))
        {
            array = new ushort[] { tileIndex };
            Tags[tag] = array;
        } 
        else
        {
            var newArray = new ushort[ array.Length + 1 ];
            Array.Copy(array, newArray, array.Length);
            newArray[ array.Length ] = tileIndex;
            Tags[tag] = newArray;
        }
    }

    public RectangleF GetUV(ushort tileIndex)
    {  
        var result =  new RectangleF(
            (_xofs + (tileIndex % _localstride)) * _uvwidth,
            (_yofs + (tileIndex / _localstride)) * _uvheight,
            _uvwidth,
            _uvheight);

        return result;
    }
}

public struct TileSetKeyframe : IKeyframe
{
    public ushort[]     Index       { get; set; }
    public ushort[]     Tiles       { get; set; }
    public float        Duration    { get; set; }
}

public class TileSetAnimationData : KeyframeAnimationData<TileSetKeyframe>
{
    protected override void UpdateFrame(AnimationChannel channel, TileSetKeyframe current)
    {
        throw new NotImplementedException();
    }
}