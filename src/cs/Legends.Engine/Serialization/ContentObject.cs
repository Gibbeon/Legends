namespace Legends.Engine.Content;

public sealed class ContentObject : INotifyReload
{
    public object Instance { get; set; }

    public ContentObject(object instance) { Instance = instance; }

    public static ContentObject Wrap<T>(T instance) { return new ContentObject(instance); }

    public void OnReload()
    {
        if(Instance is INotifyReload notify) notify.OnReload();
    }
}
