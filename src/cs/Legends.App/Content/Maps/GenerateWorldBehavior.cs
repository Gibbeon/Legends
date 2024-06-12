using Legends.Engine;
using Legends.Engine.Graphics2D.Components;
using Legends.Engine.Graphics2D.Noise;
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Legends.Engine.Input;

namespace Legends.Scripts;

public class GenerateWorldBehavior : Behavior
{
    public GenerateWorldBehavior(): base(null, null)
    {

    }
    public GenerateWorldBehavior(IServiceProvider services, SceneObject parent) : base(services, parent)
    {
        
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    float[,] _array;

    public override void Initialize()
    {
        /*
        
        var map = Parent.GetComponent<Map>();
        var size = map.TileCount;
        _array = new ImageGenerator((new Random()).Next()).GenerateHeightMap((int)size.Width + 2, (int)size.Height + 2);

        for(int row = 0; row < _array.GetLength(0) - 2; row++)
            for(int col = 0; col < _array.GetLength(1) - 2; col++)
                map.Tiles[row * (_array.GetLength(0) - 2) + col] = GetTileIndex(map, _array, row + 1, col + 1);
 
        map.BuildMap();
        InputInitialize();

        */
    }

    public override void Update(GameTime gameTime)
    {
        /*
        foreach(var command in _commands.EventActions)
        {
            switch(command.Name)
            {
                case "SELECT":
                    _tile = Parent.GetComponent<Map>().SelectTileAt(Parent.Scene.Camera.WorldToLocal(command.GetPosition()));                        
                    
                            var map = Parent.GetComponent<Map>();
                    var size = map.TileCount;
                    var tileId = map.Tiles[_tile];

                    int row1 = _tile / (int)size.Width;
                    int col1 = _tile % (int)size.Width;
                    
                    var tag = GetTag(_array, row1 + 1, col1 + 1);

                    Console.WriteLine ("tile: {0} {1}", _tile, tag);
                                    break;
                case "CHANGE_TILE":         ChangeTile(_tile, command.GetScrollDelta()); break;
                case "SET_TILE":         SetTile(); break;
                default:
                    Console.WriteLine("Unknown Command: {0}", command.Name); break;             
            }
        }  
        */
    }

    /*

    public void SetTile()
    {
        
        var map = Parent.GetComponent<Map>();
        var size = map.TileCount;
        var tileId = map.Tiles[_tile];

        int row1 = _tile / (int)size.Width;
        int col1 = _tile % (int)size.Width;

        Console.WriteLine("Getting Tag For {0}, {1} tileIndex={2}", row1 + 1, col1 + 1, GetTileIndex(map, _array, row1 + 1, col1 + 1));
        
        var tag = GetTag(_array, row1 + 1, col1 + 1);

        map.TileSet.AddTag(tag, tileId);

        Console.WriteLine("\"" + tag + "\": [" + tileId + "],");

        for(int row = 0; row < _array.GetLength(0) - 2; row++)
            for(int col = 0; col < _array.GetLength(1) - 2; col++)
                map.Tiles[row * (_array.GetLength(0) - 2) + col] = GetTileIndex(map, _array, row + 1, col + 1);


        map.BuildMap();
        
    }

    public void ChangeTile(int tile, float delta)
    {
        _delta += delta * 8;

        if(Math.Abs((int)_delta) > 0)
        {
            Parent.GetComponent<Map>().ChangeTile(tile, Parent.GetComponent<Map>().Tiles[tile] + (int)_delta);
            _delta -= (int)_delta;
        }
    }

private InputCommandSet _commands;
private int _tile;
private float _delta;
    public void InputInitialize()
    {
        _commands = new InputCommandSet(Services, Services.Get<IInputHandlerService>().Current);

        _commands.Add("SELECT", EventType.MouseClicked, MonoGame.Extended.Input.MouseButton.Left, Microsoft.Xna.Framework.Input.Keys.LeftShift);
        _commands.Add("CHANGE_TILE", EventType.MouseScroll, MonoGame.Extended.Input.MouseButton.None);
        _commands.Add("SET_TILE", EventType.MouseClicked, MonoGame.Extended.Input.MouseButton.Middle, Microsoft.Xna.Framework.Input.Keys.LeftShift);

        _commands.Enabled = true;
    }

    public string GetTag(float[,] array, int row, int col)
    {
        if(array[row, col] <= 128) return "wwwwwwwww";

        StringBuilder tag = new ("wwwwwwwww");
        int index = -1;

        for(int x = row - 1; x <= (row + 1); x++)
        {
            for(int y = col - 1; y <= (col + 1); y++)
            {
                
                index++;
                
                if(array[x, y] > 128)
                {
                    tag[index] = 'g';
                }
            }
        }

        return tag.ToString();
    }

    List<Tuple<string, int, int>> _list = new();

    public ushort GetTileIndex(Map map, float[,] array, int row, int col)
    {
        var tag = GetTag(array, row, col);

        var tile = map.TileSet.GetByTag(tag).FirstOrDefault();   
        if(tile == 0)
        {        
            _list.Add(Tuple.Create(tag, row, col));
            return 9;//map.TileSet.GetByTag(grass).FirstOrDefault();   
        }
        return tile;
    }

    public IEnumerable<Color> GetColors(Color color, float[,] array)
    {
        for(int col = 0; col < array.GetLength(1); col++)
            for(int row = 0; row < array.GetLength(0); row++)
                yield return array[col, row] < 80 ? color : Color.Black;
    }

    */

    public override void Reset()
    {

    }
}