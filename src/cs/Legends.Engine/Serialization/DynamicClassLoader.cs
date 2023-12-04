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
using Legends.Engine.Runtime;

namespace Legends.Engine.Serialization;

/// <summary>
    /// Used to load classes dynamically from C# code files.
    /// </summary>
    public static class DynamicClassLoader
    {
        // loaded assemblies
        static Dictionary<string, Assembly> _loadedAssemblies = new Dictionary<string, Assembly>();
        static Dictionary<string, byte[]> _loadedAssembliesBytes = new Dictionary<string, byte[]>();

        static bool _supportDynamicAssembly;

        /// <summary>
        /// Compile assembly, or return instance from cache.
        /// </summary>
        public static Assembly Compile(string codeIdentifier, string code)
        {
            if(!_supportDynamicAssembly)
            {
                Console.WriteLine("Setting up to _supportDynamicAssembly");

                AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => {

                    Console.WriteLine("AssemblyResolve: Trying to resolve: {0}", args.Name);
                    return _loadedAssemblies.SingleOrDefault(n => n.Value.FullName == args.Name).Value;
                };

                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += (sender, args) => {
                    Console.WriteLine("ReflectionOnlyAssemblyResolve: Trying to resolve: {0}", args.Name);
                    Assembly res;
                    _loadedAssemblies.TryGetValue(args.Name, out res);
                    return res;
                };

                _supportDynamicAssembly = true;
            }

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
                Enumerable.Concat(AppDomain.CurrentDomain.GetAssemblies()
                    .Where (n => !n.IsDynamic && File.Exists(n.Location)).Distinct()
                    .Select(n => MetadataReference.CreateFromFile(n.Location)
                ),
                
                _loadedAssembliesBytes
                    .Select(n => {
                        return MetadataReference.CreateFromImage(n.Value);
                    }
            )).ToArray();

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
                    _loadedAssembliesBytes[codeIdentifier] = ms.ToArray();
                    ret = Assembly.Load(_loadedAssembliesBytes[codeIdentifier]);
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

