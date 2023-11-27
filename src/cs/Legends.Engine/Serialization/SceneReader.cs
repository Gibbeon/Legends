using Microsoft.Xna.Framework.Content;
using Legends.Engine;
using Legends.Engine.Graphics2D;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace Legends.Engine.Serialization;

public class SpatialReader : ContentTypeReader<Spatial.SpatialDesc>
{
    protected override Spatial.SpatialDesc Read(ContentReader input, Spatial.SpatialDesc existingInstance)
    {
        var result = existingInstance ?? new Spatial.SpatialDesc();

        result.Position   = input.ReadVector2();
        result.Scale      = input.ReadVector2();
        result.Origin     = input.ReadVector2();
        result.Rotation   = input.ReadSingle();
        result.Size       = new MonoGame.Extended.Size2(input.ReadSingle(), input.ReadSingle());
        
        return result;
    }
}

public class SceneObjectReader : ContentTypeReader<SceneObject.SceneObjectDesc>
{
    protected override SceneObject.SceneObjectDesc Read(ContentReader input, SceneObject.SceneObjectDesc existingInstance)
    {
        var result = existingInstance ?? new SceneObject.SceneObjectDesc();
        
        input.ReadRawObject<Spatial.SpatialDesc>(result);

        result.Name = input.ReadString();
        result.Enabled = input.ReadBoolean();
        result.IsVisible = input.ReadBoolean();

        var numTags = input.ReadInt32();
        for(int i = 0; i < numTags; i++)
        {
            result.Tags.Add(input.ReadString());
        }

        var numChildren = input.ReadInt32();
        for(int i = 0; i < numChildren; i++)
        {
            result.Children.Add(input.ReadObject<SceneObject.SceneObjectDesc>());
        }

        var numBehaviors = input.ReadInt32();
        for(int i = 0; i < numBehaviors; i++)
        {
            result.Behaviors.Add(input.ReadObject<IBehavior.BehaviorDesc>());
        }

        return result;
    }
}

public class SceneReader : ContentTypeReader<Scene.SceneDesc>
{
    protected override Scene.SceneDesc Read(ContentReader input, Scene.SceneDesc existingInstance)
    {
        var result = existingInstance ?? new Scene.SceneDesc();        
        input.ReadRawObject<SceneObject.SceneObjectDesc>(result); 

        result.Camera = input.ReadObject<Camera.CameraDesc>();       

        return result;
    }
}

public class CameraReader : ContentTypeReader<Camera.CameraDesc>
{
    protected override Camera.CameraDesc Read(ContentReader input, Camera.CameraDesc existingInstance)
    {
        var result = existingInstance ?? new Camera.CameraDesc();        
        input.ReadRawObject<SceneObject.SceneObjectDesc>(result);       

        return result;
    }
}