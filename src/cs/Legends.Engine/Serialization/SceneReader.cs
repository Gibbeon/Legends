using Microsoft.Xna.Framework.Content;
using System.Linq;
using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Legends.Engine.Graphics2D;
using MonoGame.Extended;
using Microsoft.CodeAnalysis.CSharp;

using Microsoft.CodeAnalysis;
using System.IO;
using Microsoft.CodeAnalysis.Emit;

namespace Legends.Engine.Serialization;

/// <summary>
    /// Used to load classes dynamically from C# code files.
    /// </summary>
    public static class DynamicClassLoader
    {
        // loaded assemblies
        static Dictionary<string, Assembly> _loadedAssemblies = new Dictionary<string, Assembly>();

        /// <summary>
        /// Compile assembly, or return instance from cache.
        /// </summary>
        public static Assembly Compile(string codeIdentifier, string code)
        {
            // get from cache
            Assembly ret = null;
            if (_loadedAssemblies.TryGetValue(codeIdentifier, out ret))
            {
                return ret;
            }

            // create syntax tree from code
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);

            //The location of the .NET assemblies
            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);

            // get randomly generated assembly name
            string assemblyName = Path.GetRandomFileName();
            
            /*
            MetadataReference[] references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.GetExecutingAssembly().Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "mscorlib.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Core.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Collections.dll")),
            };
            */

            MetadataReference[] references = 
            AppDomain.CurrentDomain.GetAssemblies()
                .Where (n => !n.IsDynamic && File.Exists(n.Location)).Distinct()
                .Select(n => MetadataReference.CreateFromFile(n.Location)
            ).ToArray();

            // setup compiler
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            // compile code
            using (var ms = new MemoryStream())
            {
                // first compile
                EmitResult result = compilation.Emit(ms);

                // if failed, get error message
                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        throw new Exception(string.Format("Failed to compile code '{0}'! {1}: {2}", codeIdentifier, diagnostic.Id, diagnostic.GetMessage()));
                    }
                    throw new Exception("Unknown error while compiling code '" + codeIdentifier + "'!");
                }
                // on success, load into assembly
                else
                {
                    // load assembly and add to cache
                    ms.Seek(0, SeekOrigin.Begin);
                    ret = Assembly.Load(ms.ToArray());
                    _loadedAssemblies[codeIdentifier] = ret;

                    // return assembly
                    return ret;
                }
            }
        }

        /// <summary>
        /// Compile code and extract a class from it.
        /// </summary>
        /// <param name="path">Script file path.</param>
        /// <param name="className">Class name to get.</param>
        public static Type CompileCodeAndExtractClass(string path, string className)
        {
            return CompileCodeAndExtractClass(path, File.ReadAllText(path), className);
        }

        /// <summary>
        /// Compile code and extract a class from it.
        /// </summary>
        /// <param name="codeIdentifier">Unique identifier for the code.</param>
        /// <param name="code">Code to compile.</param>
        /// <param name="className">Class name to extract. Can be null to just load and compile the code.</param>
        public static Type CompileCodeAndExtractClass(string codeIdentifier, string code, string className)
        {
            // if debug mode and class is part of project, return as-is
#if DEBUG
            if (className == null) { return null; }
            var debugRet = Assembly.GetExecutingAssembly().GetType(className);
            if (debugRet != null)
            {
                return debugRet;
            }
#endif
            // return value
            Type ret = null;

            // extract class
            if (!string.IsNullOrEmpty(className))
            {
                // get assembly and try to extract class
                var assembly = Compile(codeIdentifier, code);
                ret = assembly.GetType(className);

                // didn't find main class?
                if (ret == null)
                {
                    throw new Exception("Fail to find class '" + className + "' in code '" + codeIdentifier + "'!");
                }
            }

            // return class
            return ret;
        }
    }

public static class ContentReaderExtensions
{
    private static int Indent;

    private static int IndentSpaces = 2;

    public static bool OutputToConsole = true;

    public static bool InitFile;

    public static int? _offset;

    public static void Log<TType>(this ContentReader input, string message, params object?[]? args)
    {
        if(OutputToConsole)
        {
            if(_offset == null) _offset = (int)input.BaseStream.Position - 1;

            Console.Write("{0,8} ", (input.BaseStream.Position - (int)_offset).ToString("D8"));
            if(Indent * IndentSpaces > 0)
            {
                Console.Write(new string(Enumerable.Repeat(' ', Indent * IndentSpaces).ToArray()));
            }
            Console.Write("{0}: ", typeof(TType).Name);
            Console.WriteLine(message, args);
        }
    }

    private static MethodInfo _readRawObject;
    private static MethodInfo[] _contentReaderReadMethods;
    public static void ReadBaseObject<TType>(this ContentReader input, TType result)
    {

        if(typeof(TType).BaseType != null && typeof(TType).BaseType != typeof(object))
        {        
            input.Log<TType>("Reading Base Type: {0}", typeof(TType).BaseType.Name);
            Indent++;
            
            _readRawObject = _readRawObject ?? 
                typeof(ContentReader).GetMethods().Single(n => n.Name == "ReadRawObject" && n.IsGenericMethod && n.GetParameters().Length == 1 && n.GetParameters()[0].ParameterType.IsGenericParameter);
            _readRawObject.MakeGenericMethod(typeof(TType).BaseType)?.Invoke(input, new object[] { result });
            //input.ReadRawObject<SceneObject>(result);
            
            Indent--;
        }
    }

    static Size2 ReadSize2(this ContentReader input)
    {
        return new Size2(input.ReadSingle(), input.ReadSingle());
    }

    static Asset<TType> ReadAsset<TType>(this ContentReader input)
    {
        return new Asset<TType>(input.ReadString());
    }

    static IEnumerable<MethodInfo> GetExtensionMethods(Type extendedType, Assembly? assembly = default)
    {
        assembly = assembly ?? typeof(ContentReaderExtensions).Assembly;
        var isGenericTypeDefinition = extendedType.IsGenericType && extendedType.IsTypeDefinition;
        var query = from type in assembly.GetTypes()
            where type.IsSealed && !type.IsGenericType && !type.IsNested
            from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            where method.IsDefined(typeof(ExtensionAttribute), false)
            where isGenericTypeDefinition
                ? method.GetParameters()[0].ParameterType.IsGenericType && method.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == extendedType
                : method.GetParameters()[0].ParameterType == extendedType
            select method;
        return query;
    }

    public static object? Create(Type type, params object?[]? objects)
    {
        foreach(var ctor in type.GetConstructors().Where(n => n.GetParameters().Length <= objects?.Length).OrderBy(n => n.GetParameters().Length))
        {
            var ctorParamList = ctor.GetParameters();
            var typeParams = new object[ctorParamList.Length]; 
            for(int i = 0; i < ctorParamList.Length; i++)
            {
                if(typeParams[i] == null)
                {
                    typeParams[i] = objects?.SingleOrDefault(n => n.GetType().IsAssignableTo( ctorParamList[i].ParameterType));
                }
            }

            if(typeParams.All(n => n != null))
            {
                return (object?)Activator.CreateInstance(type, typeParams);
            }
        }

        return null;
    }

    public static TType? Create<TType>(params object?[]? objects)
    {
        return (TType?)Create(typeof(TType), objects);
    }

    public static void ReadFields<TType>(this ContentReader input, TType value)
    {
        var fields = typeof(TType).GetFields(
            BindingFlags.DeclaredOnly |
            BindingFlags.Public |  
            BindingFlags.Instance);

        var properties = typeof(TType).GetProperties(
            BindingFlags.DeclaredOnly |
            BindingFlags.Public |  
            BindingFlags.Instance);

        _contentReaderReadMethods = _contentReaderReadMethods ?? Enumerable.Concat(input.GetType().GetMethods(
                                                                        BindingFlags.Public |  
                                                                        BindingFlags.Instance)
                                                                        .Where(n => n.Name.StartsWith("Read")),
                                                                        GetExtensionMethods(input.GetType()).Where(n => n.Name.StartsWith("Read"))).ToArray();

        foreach(var field in fields)
        {

        }

        foreach(var property in properties)
        {
            if(property.CustomAttributes.Any(n => n.AttributeType == typeof(JsonIgnoreAttribute)))
            {
                input.Log<TType>("!!Ignoring Property: {0}", property.Name);
                continue;
            }

            input.Log<TType>("Reading Property: {0}", property.Name);
            var readMethod = _contentReaderReadMethods.SingleOrDefault(
                    n => 
                    !n.IsStatic && n.ReturnType.IsAssignableTo(property.PropertyType)
                    || n.IsStatic && n.ReturnType.IsAssignableTo(property.PropertyType)
                    || property.PropertyType.IsGenericType && n.ReturnType.IsGenericType && n.ReturnType.GetGenericTypeDefinition().MakeGenericType(property.PropertyType.GenericTypeArguments).IsAssignableTo(property.PropertyType));

            if(readMethod != null)
            {
                if(readMethod.IsGenericMethod && readMethod.IsStatic)
                {
                    var genericValue = readMethod.MakeGenericMethod(property.PropertyType.GenericTypeArguments).Invoke(null, new object?[] { input });
                    property.SetValue(value, genericValue);
                }
                else if(!readMethod.IsStatic)
                {
                    var instanceReadValue = readMethod.Invoke(input, null);
                    property.SetValue(value, instanceReadValue);
                } 
                else
                {
                    var staticReadValue = readMethod.Invoke(null, new object?[] { input });
                    property.SetValue(value, staticReadValue);
                }

                input.Log<TType>("..Ordinal: {0} = {1}", property.PropertyType.Name, property.GetValue(value));
            }
            else if(property.PropertyType.IsEnum)
            {
                property.SetValue(value, Enum.Parse(property.PropertyType, input.ReadString()));
                
                input.Log<TType>("..Enum: {0} = {1}", property.PropertyType.Name, property.GetValue(value));
            }
            else if(property.PropertyType.IsArray || property.PropertyType.GetInterface(typeof(IEnumerable).Name) != null)
            {  
                int count = input.ReadInt32();
                
                input.Log<TType>("..Array [{0}] of Size {1} ", property.PropertyType.Name, count);

                var itemType = property.PropertyType.IsArray ? property.PropertyType.GetElementType() :
                    property.PropertyType.GenericTypeArguments[0];

                var list = property.GetValue( value );

                if(list == null)
                {
                    if(property.PropertyType.IsArray)
                    {
                        list = Array.CreateInstance(itemType, count);
                    }
                    else
                    {
                        list = Activator.CreateInstance(property.PropertyType);
                    }

                    property.SetValue(value, list);
                }

                for(int i = 0; i < count; i++)
                {
                    readMethod = _contentReaderReadMethods.SingleOrDefault(
                                n => !n.IsStatic && n.ReturnType.IsAssignableTo(itemType)
                                || n.IsStatic && n.ReturnType.IsAssignableTo(itemType));

                    if(readMethod == null)
                    {
                        readMethod = _contentReaderReadMethods.Single(n => n.Name == "ReadObject" && n.IsGenericMethod && n.GetParameters().Length == 0).MakeGenericMethod(itemType);
                    }

                    var itemValue = readMethod.Invoke(input, null);

                    if(list.GetType().IsArray)
                    {
                        ((Array)list).SetValue(itemValue, i);
                    }
                    else
                    {
                        var addMethod = typeof(ICollection<>).MakeGenericType(itemType).GetMethod("Add");
                        addMethod.Invoke(list, new object?[] { itemValue });
                    }
                    
                    input.Log<TType>("..Ordinal [{0}]: {1}", i, itemValue?.ToString());
                }
            }
            else
            {
                readMethod = _contentReaderReadMethods.Single(n => n.Name == "ReadObject" && n.IsGenericMethod && n.GetParameters().Length == 0).MakeGenericMethod(property.PropertyType);
                
                var readObject = readMethod.Invoke(input, null);
                property.SetValue(value, readObject);
                
                input.Log<TType>("..Reading Object of Type {0} = {1}", property.PropertyType.Name, readObject);
            }
        }
    }
}

/*public class SceneGenericReader : ContentTypeReader<Scene>
{
    protected override Scene Read(ContentReader input, Scene existingInstance)
    {
        var result = existingInstance ?? new Scene(input.ContentManager.ServiceProvider);

        input.ReadBaseObject(result);
        input.ReadFields(result);

        return result;
    }
}

public class SceneObjectGenericReader : ContentTypeReader<SceneObject>
{
    protected override SceneObject Read(ContentReader input, SceneObject existingInstance)
    {
        var result = existingInstance ?? new SceneObject(input.ContentManager.ServiceProvider);

        input.ReadBaseObject(result);
        input.ReadFields(result);

        return result;
    }
}

public class SpatialGenericReader : ContentTypeReader<Spatial>
{
    protected override Spatial Read(ContentReader input, Spatial existingInstance)
    {
        var result = existingInstance ?? new Spatial();

        input.ReadBaseObject(result);
        input.ReadFields(result);

        return result;
    }
}

public class CameraGenericReader : ContentTypeReader<Camera>
{
    protected override Camera Read(ContentReader input, Camera existingInstance)
    {
        var result = existingInstance ?? new Camera(input.ContentManager.ServiceProvider, null);

        input.ReadBaseObject(result);
        input.ReadFields(result);

        return result;
    }
}

public class TextRenderBehaviorReader : ContentTypeReader<TextRenderBehavior>
{
    protected override TextRenderBehavior Read(ContentReader input, TextRenderBehavior existingInstance)
    {
        var result = existingInstance ?? new TextRenderBehavior(input.ContentManager.ServiceProvider, null);

        input.ReadBaseObject(result);
        input.ReadFields(result);

        return result;
    }
}*/


public class GenericReader<TType> : ContentTypeReader<TType>
{
    protected override TType Read(ContentReader input, TType existingInstance)
    {
        var result = existingInstance ?? 
            ContentReaderExtensions.Create<TType>(new object?[] { input.ContentManager.ServiceProvider });

            //(//TType)Activator.CreateInstance(typeof(TType), new object?[] { input.ContentManager.ServiceProvider, null });

        input.ReadBaseObject(result);
        input.ReadFields(result);

        return result;
    }
}


/*
public class SpatialReader : ContentTypeReader<Spatial.SpatialDesc>
{
    protected override Spatial.SpatialDesc Read(ContentReader input, Spatial.SpatialDesc existingInstance)
    {
        var result = existingInstance ?? new Spatial.SpatialDesc();

        result.Position   = input.ReadVector2();
        result.Scale      = input.ReadVector2();
        result.Origin     = input.ReadVector2();
        result.Rotation   = input.ReadSingle();
        result.Size       = new MonoGame.Extended.Size2(input.ReadSingle(), input.ReadSingle());
        
        return result;
    }
}

public class SceneObjectReader : ContentTypeReader<SceneObject.SceneObjectDesc>
{
    protected override SceneObject.SceneObjectDesc Read(ContentReader input, SceneObject.SceneObjectDesc existingInstance)
    {
        var result = existingInstance ?? new SceneObject.SceneObjectDesc();
        
        input.ReadRawObject<Spatial.SpatialDesc>(result);

        result.Name = input.ReadString();
        result.Enabled = input.ReadBoolean();
        result.IsVisible = input.ReadBoolean();

        var numTags = input.ReadInt32();
        for(int i = 0; i < numTags; i++)
        {
            result.Tags.Add(input.ReadString());
        }

        var numChildren = input.ReadInt32();
        for(int i = 0; i < numChildren; i++)
        {
            result.Children.Add(input.ReadObject<SceneObject.SceneObjectDesc>());
        }

        var numBehaviors = input.ReadInt32();
        for(int i = 0; i < numBehaviors; i++)
        {
            result.Behaviors.Add(input.ReadObject<IBehavior.BehaviorDesc>());
        }

        return result;
    }
}

public class SceneReader : ContentTypeReader<Scene.SceneDesc>
{
    protected override Scene.SceneDesc Read(ContentReader input, Scene.SceneDesc existingInstance)
    {
        var result = existingInstance ?? new Scene.SceneDesc();        
        input.ReadRawObject<SceneObject.SceneObjectDesc>(result); 

        result.Camera = input.ReadObject<Camera.CameraDesc>();       

        return result;
    }
}

public class CameraReader : ContentTypeReader<Camera.CameraDesc>
{
    protected override Camera.CameraDesc Read(ContentReader input, Camera.CameraDesc existingInstance)
    {
        var result = existingInstance ?? new Camera.CameraDesc();        
        input.ReadRawObject<SceneObject.SceneObjectDesc>(result);       

        return result;
    }
}

public class TextRenderBehaviorReader : ContentTypeReader<TextRenderBehavior.TextRenderBehaviorDesc>
{
    protected override TextRenderBehavior.TextRenderBehaviorDesc Read(ContentReader input, TextRenderBehavior.TextRenderBehaviorDesc existingInstance)
    {
        var result = existingInstance ?? new TextRenderBehavior.TextRenderBehaviorDesc();        
        input.ReadRawObject<ActivatorDesc>(result); 
        result.Text     = input.ReadString();
        result.Color    = input.ReadColor();
        result.Font     = input.ReadString();      

        return result;
    }
}
*/