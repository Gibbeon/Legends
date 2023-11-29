using Microsoft.Xna.Framework.Content;

namespace Legends.Engine.Serialization;

public class GenericReader<TType> : ContentTypeReader<TType>
{
    protected override TType Read(ContentReader input, TType existingInstance)
    {
        return existingInstance;
    }
}

/*
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

public class TextRenderBehaviorReader : ContentTypeReader<TextRenderBehavior.TextRenderBehaviorDesc>
{
    protected override TextRenderBehavior.TextRenderBehaviorDesc Read(ContentReader input, TextRenderBehavior.TextRenderBehaviorDesc existingInstance)
    {
        var result = existingInstance ?? new TextRenderBehavior.TextRenderBehaviorDesc();        
        input.ReadRawObject<ActivatorDesc>(result); 
        result.Text     = input.ReadString();
        result.Color    = input.ReadColor();
        result.Font     = input.ReadString();      

        return result;
    }
}
*/