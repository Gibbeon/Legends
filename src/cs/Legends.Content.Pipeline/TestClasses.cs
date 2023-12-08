
using System.Collections.Generic;
using Legends.Engine.Content;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Legends.Content.Pipeline;


public interface IBehaviorLike
{
    public string Duval { get; }
}
public class BehaviorLike : IBehaviorLike
{
    public string Duval { get; set; }
    public Asset<Texture2D> Texture { get; set; }  
}

public sealed class ContentObject
{
    public object Instance { get; set; }

    public ContentObject(object instance) { Instance = instance; }

    public static ContentObject Wrap<T>(T instance) { return new ContentObject(instance); }
}

public class SceneLike
{
    public string Name { get; set; }
    public Asset<Texture2D> Texture { get; set; }  
    public List<Scriptable<IBehaviorLike>> Behaviors { get; set; }
    private Texture2D SourceData => ~Texture;

    public void Update()
    {
        
    }
}

public class TesterClass
{
    private Asset<SceneLike> _scene;
    public ContentManager Content;
    public TesterClass(ContentManager content)
    {
        Content = content;
        content.Load("Scenes/test", out _scene);
    }

    public void Update()
    {
        (~_scene).Update();
    }
}