using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Newtonsoft.Json;

namespace Legends.Engine.Content;

public static class ContentHelpers
{
    public static IEnumerable<MemberInfo> GetContentMembers(Type derivedType)
    {
        return Enumerable.Concat<MemberInfo>(
                    derivedType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty)
                        .Where(n => 
                            n.CanRead && 
                            n.CanWrite &&
                            (
                                n.GetAccessors().Any(n => n.IsPublic) ||
                                n.IsDefined(typeof(JsonPropertyAttribute))
                            )
                    ),
                    derivedType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        .Where(n => n.IsPublic ||
                                    n.IsDefined(typeof(JsonPropertyAttribute))))
                    .Where(
                        n => !n.IsDefined(typeof(JsonIgnoreAttribute))
                    );
    }
}
