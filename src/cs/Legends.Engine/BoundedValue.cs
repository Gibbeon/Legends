using System;

public struct BoundedValue<TType>
    where TType : IComparable<TType>
{
    public TType Minimum { get; set; }
    public TType Maximum { get; set; }

    private readonly bool _enabled;

    public BoundedValue(TType minimum, TType maximum)
    {
        Minimum = minimum;
        Maximum = maximum;
        _enabled = true;
    }

    public readonly TType GetValue(TType newValue)
    {
        if(_enabled && newValue.CompareTo(Minimum) <= 0) return Minimum;
        if(_enabled && newValue.CompareTo(Maximum) >= 0) return Maximum;
        
        return newValue;
    }
}
