using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.Sprites;
using System;
using System.Net.Http.Headers;
using System.Xml.Linq;
using MonoGame.Extended;
using MonoGame.Framework.Utilities;
using System.Runtime.InteropServices;

namespace Legends.Engine;

public class GraphicsDeviceService : IGraphicsDeviceService
{
    private GraphicsDeviceManager _manager;

    public GraphicsDevice GraphicsDevice => _manager.GraphicsDevice;

    public event EventHandler<EventArgs> DeviceCreated { add => _manager.DeviceCreated += value; remove => _manager.DeviceCreated -= value; }

    public event EventHandler<EventArgs> DeviceDisposing { add => _manager.DeviceDisposing += value; remove => _manager.DeviceDisposing -= value; }

    public event EventHandler<EventArgs> DeviceReset { add => _manager.DeviceReset += value; remove => _manager.DeviceReset -= value; }

    public event EventHandler<EventArgs> DeviceResetting { add => _manager.DeviceResetting += value; remove => _manager.DeviceResetting -= value; }


    public GraphicsDeviceService(SystemServices _service)
    {
        _manager = new GraphicsDeviceManager(_service.Game);
    }
}
