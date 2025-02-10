using System;
using System.Linq.Expressions;

namespace Legends.Engine.Animation;

public class LerpProgress<TType>
{    
    private static Func<TType, TType, float, TType> _defaultLerp;
    private Func<TType, TType, float, TType> _lerp;    

    public TType From     { get; set; }
    public TType To       { get; set; }
    public TType Value    { get => Lerp(From, To, Percentage); }
    public Func<TType, TType, float, TType> Lerp { get => _lerp ?? GetDefaultLerp(); set => _lerp = value; }

    public float Percentage;

    public static Func<TType, TType, float, TType> GetDefaultLerp()
    {
        return _defaultLerp ?? (_defaultLerp = GenerateDefaultLerp());
    }
    public static Func<TType, TType, float, TType> GenerateDefaultLerp()
    {
        var method = typeof(TType)
            .GetMethod("Lerp", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        
        if(method != null)
        {
            return method.CreateDelegate<Func<TType, TType, float, TType>>();
        }
        else
        {
            var value1 = Expression.Parameter(typeof(TType));
            var value2 = Expression.Parameter(typeof(TType));
            var amount = Expression.Parameter(typeof(float));

            return Expression.Lambda<Func<TType, TType, float, TType>>(
                Expression.Convert(Expression.Multiply(Expression.Convert(Expression.Add(value1, Expression.Subtract(value2, value1)), typeof(float)), amount), typeof(TType)),
                value1, 
                value2, 
                amount
            ).Compile();
        }
    }
}