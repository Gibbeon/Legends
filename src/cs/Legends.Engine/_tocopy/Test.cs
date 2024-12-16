class Resource
{
    string Name { get; }
    bool IsExternal { get; }
}

class Texture
{  
    public void Texture(Resource reference)
    {
        reference.Read<Texture2D>(this);
    }

    public void Dispose()
    {

    }

    //public static explicit operator Texture(Texture2D b) => new Digit(b);
}

struct Moniker
{
    private string _name;
    public string Name { get => _name; }

    public static implicit operator Moniker(string name) => new Moniker(name);
    
    public static implicit operator string(Moniker moniker) => moniker.Name;

    public override string ToString() => Name;
}

class ContentManager
{
    public Dictionary<string, Resource> _cache;

    public Resource Get(Moniker name)
    {        
        if(_cache.TryGetValue(name, out var resource))
        {
            return resource;
        }

        return Load(name);
    }

    protected Resource Load(Moniker name, Resource resource, bool cache = true)
    {
        var resource = default(Resource);// cm.Load<object>(name);
        return _cache[name] = resource;
    }

    public OnChange(Resource resource)
    {
        if(_resources.ContainsKey(resource.Name))
        ContentManager.Load(resource.Name, object? existing);
    }
}