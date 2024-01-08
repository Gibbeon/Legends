namespace Legends.Engine.Content;

public sealed class ContentObject : IResetable
{
    public object Instance { get; set; }

    public ContentObject(object instance) { Instance = instance; }

    public static ContentObject Wrap<T>(T instance) { return new ContentObject(instance); }

    public void Reset()
    {
        if(Instance is IInitalizable notify) notify.Reset();
    }
}
