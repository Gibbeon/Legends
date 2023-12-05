using Microsoft.Xna.Framework;
using Legends.Engine;
using System;
using MonoGame.Extended;
using System.IO;

namespace Legends.Engine;

public interface IDynamicallyCompiledType
{
    public string Source { get; set; }
    public string TypeName { get; set; }
    public dynamic Properties { get; set; }
}

public class ScriptBehavior : BaseBehavior, IDynamicallyCompiledType
{
    public string Source { get; set; }
    public string TypeName { get; set; }
    public dynamic Properties { get; set; }

    public ScriptBehavior() : this(null, null)
    {

    }

    public ScriptBehavior(IServiceProvider services, SceneObject parent) : base(services, parent)
    {

    }

    public override void Update(GameTime gameTime)
    {
       throw new NotImplementedException();
    }

    public override void Dispose()
    {
        
    }
}