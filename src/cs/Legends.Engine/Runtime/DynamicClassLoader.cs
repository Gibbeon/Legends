using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;

using Microsoft.CodeAnalysis;
using System.IO;
using Microsoft.CodeAnalysis.Emit;
using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace Legends.Engine.Serialization;

public static class DynamicClassLoader
{
    static readonly Dictionary<string, DynamicAssembly> _assemblyCache;
    static readonly MetadataReference[] _globalReferences;
    public static bool IsInitialized { get; private set; }
    public static DynamicAssembly Compile(string identifier, string code, bool generateSymbols = true, bool useCache = true)
    {
        if(useCache && _assemblyCache.TryGetValue(identifier, out DynamicAssembly dynamicAssembly))
        {
            return dynamicAssembly;
        }
            // create syntax tree from code
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code, 
            null,
            Path.GetFullPath(identifier),
            Encoding.UTF8);

        //The location of the .NET assemblies
        var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);

        // get randomly generated assembly name
        string assemblyName = Path.ChangeExtension(Path.GetRandomFileName(), "dll");
        string symbolsName  = Path.ChangeExtension(assemblyName, "pdb");

        var buffer      = Encoding.UTF8.GetBytes(code);
        var sourceText  = SourceText.From(buffer, buffer.Length, Encoding.UTF8, canBeEmbedded: true);

        var references = Enumerable.Concat(_globalReferences, _assemblyCache.Values.Select(n => n.MetadataReference)).ToArray();

    // setup compiler
        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName,
            syntaxTrees: new[] { syntaxTree },
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                            .WithOptimizationLevel(generateSymbols ? OptimizationLevel.Debug : OptimizationLevel.Release)
                            .WithPlatform(Platform.AnyCpu));


        using (var assemblyStream = new MemoryStream())
        {
            var emitOptions = new EmitOptions(
                    debugInformationFormat: DebugInformationFormat.Embedded,
                    pdbFilePath: symbolsName);

            var embeddedTexts = new List<EmbeddedText>
            {
                EmbeddedText.FromSource(identifier, sourceText),
            };

            EmitResult result = compilation.Emit(
                peStream: assemblyStream,
                embeddedTexts: embeddedTexts,
                options: generateSymbols ? emitOptions : new EmitOptions());

            if (!result.Success)
            {
                var errors = new List<string>();

                IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                    diagnostic.IsWarningAsError ||
                    diagnostic.Severity == DiagnosticSeverity.Error);

                foreach (Diagnostic diagnostic in failures)
                    errors.Add($"{diagnostic.Id}: {diagnostic.GetMessage()}");

                throw new Exception(String.Join("\n", errors));
            }

            assemblyStream.Seek(0, SeekOrigin.Begin);
            return Load(identifier, assemblyStream.ToArray());
        }
    }

    public static bool Contains(Assembly assembly)
    {
        return _assemblyCache.Values.Select(n => n.Assembly).Contains(assembly);
    }

    public static Type GetType(string typeName)
    {
        return _assemblyCache
                    .Values
                    .Select(n => n.Assembly.GetType(typeName))
                    .FirstOrDefault(n => n != null);
    }

    public static string GetAssetName(Assembly assembly)
    {
        return _assemblyCache
                    .Where(n => Equals(n.Value.Assembly, assembly))
                    .Select(n => n.Key)
                    .SingleOrDefault();
    }

    public static void Register(DynamicAssembly dynamicAssembly)
    {
        _assemblyCache[dynamicAssembly.Identifier] = dynamicAssembly;
    }

    public static DynamicAssembly Load(string identifier, byte[] rawData)
    { 
        if(_assemblyCache.TryGetValue(identifier, out DynamicAssembly dynamicAssembly))
        {
            return dynamicAssembly;
        }

        dynamicAssembly = new DynamicAssembly(identifier, rawData);

        Register(dynamicAssembly);

        return dynamicAssembly;
    }

    static DynamicClassLoader()
    {
        if(!IsInitialized)
        {
            _assemblyCache = new();
            _globalReferences = BuildReferences(Assembly.GetEntryAssembly(), Assembly.GetCallingAssembly()).ToArray();
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += AssemblyResolve;
            IsInitialized = true;
        }
    }

    private static IEnumerable<MetadataReference> BuildReferences(params Assembly[] assemblies)
    {
        Stack<Assembly> toLoad = new(assemblies);

        while(toLoad.Count > 0)
        {
            foreach(var assemblyRef in toLoad.Pop().GetReferencedAssemblies())
            {
                if(AppDomain.CurrentDomain.GetAssemblies().Any(n => assemblyRef.FullName == n.FullName)) continue;

                try
                {
                    toLoad.Push(Assembly.Load(assemblyRef));
                } 
                catch
                {
                    
                }
            }
        }
        
        return AppDomain.CurrentDomain.GetAssemblies()
                .Where (n => !n.IsDynamic && File.Exists(n.Location)).Distinct()
                .Select(n => MetadataReference.CreateFromFile(n.Location));
    }

    private static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
    {
        return _assemblyCache.Values.SingleOrDefault(n => n.Assembly.FullName == args.Name)?.Assembly;
    }
}

