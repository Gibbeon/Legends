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

    public SpatialNode(SpatialNode parent) : this(new SpatialNodeDesc())
    {
        parent.Attach(this);
    }

    public SpatialNode(SpatialNodeDesc data) : base(data)
    {
        _children = new List<Spatial>();
    }

    public override void SetPosition(Vector2 position)
    {
        base.SetPosition(position);

        if(_children != null)
        {
            foreach(var item in _children)
            {
                item.OffsetPosition = position;
            }
        }   
    }
    public override void SetScale(Vector2 scale)
    {
        base.SetScale(scale);

        if(_children != null)
        {
            foreach(var item in _children)
            {
                item.OffsetScale = scale;
            }
        }   
    }
    public override void SetRotation(float radians)
    {
        base.SetRotation(radians);

        if(_children != null)
        {
            foreach(var item in _children)
            {
                item.OffsetRotation = radians;
            }
        }   
    }

    public void Attach(Spatial node, bool relative = false)
    {
        if(relative)
        {
            node.OffsetPosition   = this.Position;
            node.OffsetRotation   = this.Rotation;
            node.OffsetScale      = this.Scale;
        }

        _children.Add(node);
    }

    public void Detach(Spatial node, bool relative = false)
    {
        if(relative)
        {
            node.OffsetPosition   = this.Position;
            node.OffsetRotation   = this.Rotation;
            node.OffsetScale      = this.Scale;
        }

        _children.Remove(node);
    }
}  