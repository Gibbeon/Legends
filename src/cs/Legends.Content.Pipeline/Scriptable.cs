using System;

namespace Legends.Engine.Content;

public interface IScriptable
{
    string Source { get; }
    Type AssetType { get; }
    string TypeName { get; }
    dynamic Properties { get; }
}

public class Scriptable<TType> : Asset<TType>, IScriptable
{
    public string TypeName { get; set; }
    public dynamic Properties { get; set; }
    public Scriptable(string name) : base(name) {}

    public override string ToString()
    {
        return string.Format("TypeName: {0} Source: {1}", TypeName, Source);
    }
}