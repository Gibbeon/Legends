using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace SlyEngine.Graphics2D;
public class SpatialNode : Spatial
{   
    public class SpatialNodeDesc : SpatialDesc
    {

    }

    private List<Spatial> _children;

    public IReadOnlyList<Spatial> Children
    {
        get => _children.AsReadOnly();
    }

    public SpatialNode() : this(new SpatialNodeDesc())
    {

    }

    public SpatialNode(SpatialNodeDesc data) : base(data)
    {
        _children = new List<Spatial>();
    }

    public override void SetPosition(Vector2 position)
    {
        _children?.Select(n => n.Position += (position - Position));

        base.SetPosition(position);
    }
    public override void SetScale(Vector2 scale)
    {
        _children?.Select(n => n.Scale += (scale - Scale));

        base.SetScale(scale);
    }
    public override void SetRotation(float radians)
    {
        _children?.Select(n => n.Rotation += (radians - Rotation));

        base.SetRotation(radians);
    }

    public void Add(Spatial node, bool relative = false)
    {
        if(relative)
        {
            node.Position   += this.Position;
            node.Rotation   += this.Rotation;
            node.Scale      += this.Scale;
        }

        _children.Add(node);
    }

    public void Remove(Spatial node, bool relative = false)
    {
        if(relative)
        {
            node.Position   -= this.Position;
            node.Rotation   -= this.Rotation;
            node.Scale      -= this.Scale;
        }

        _children.Remove(node);
    }
}  