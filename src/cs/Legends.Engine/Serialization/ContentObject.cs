namespace Legends.Engine.Content;

public sealed class ContentObject
{
    public object Instance { get; set; }

    public ContentObject(object instance) { Instance = instance; }

    public static ContentObject Wrap<T>(T instance) { return new ContentObject(instance); }
}
