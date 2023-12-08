
using System.Collections.Generic;
using Legends.Engine.Content;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Legends.Content.Pipeline;

public interface IContentObject
{

}

public interface IBehaviorLike
{
    public string Duval { get; }
}
public class BehaviorLike : IBehaviorLike
{
    public string Duval { get; set; }
    public Asset<Texture2D> Texture { get; set; }  
}

public class SceneLike : IContentObject
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