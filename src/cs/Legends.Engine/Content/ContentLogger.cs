using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using Legends.Engine.Runtime;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Newtonsoft.Json;

namespace Legends.Engine.Content;

public static class ContentLogger
{
    public static bool Enabled { get; set; }
    private static int Indent;
    private const int IndentSpaces = 2;

    public class IndentContext : IDisposable { public IndentContext() { Indent++;} public void Dispose() { Indent--; } }

    public static IndentContext LogBegin(long filePos, string message, params object[] args)
    {
        if(Enabled) Console.Write("{0:D8} ", filePos);
        if(Enabled && Indent > 0) Console.Write(new string(Enumerable.Repeat(' ', IndentSpaces * Indent).ToArray()));
        var result = new IndentContext();
        if(string.IsNullOrEmpty(message)) return result;
        if(Enabled) Console.Write(message, args);
        return result;
    }

    public static IndentContext Log(long filePos, string message, params object[] args)
    {
        var result = LogBegin(filePos, message, args);
        if(Enabled) Console.WriteLine();
        return result;
    }

    public static void LogAppend(string message, params object[] args)
    {
        if(string.IsNullOrEmpty(message)) return;
        if(Enabled) Console.Write(" " + message, args);
    }

    public static void LogEnd(string message, params object[] args)
    {
        if(Enabled) Console.WriteLine(" " + message, args);
    }
}