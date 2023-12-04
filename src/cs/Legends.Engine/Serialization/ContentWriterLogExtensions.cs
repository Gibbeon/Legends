using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using System;
using Newtonsoft.Json;
using System.IO;
using MonoGame.Extended;
using Legends.Engine.Runtime;
using Legends.Engine.Graphics2D;

namespace Legends.Content.Pipline;

public static class ContentWriterLogExtensions
{
    private static int Indent;
    
    private static int IndentSpaces = 2;

    public static bool OutputToConsole = true;

    public class LogContext : IDisposable
    {
        public LogContext()
        {
            Indent++;
        }
        public void Dispose()
        {
            Indent--;
        }
    }

    public static LogContext LogEntry<TType>(this ContentWriter output, string message, params object?[]? args)
    {
        output.Log<TType>(message, args);

        return new LogContext();
    }

    public static void Log<TType>(this ContentWriter output, string message, params object?[]? args)
    {
        if(OutputToConsole)
        {
            var pos = output.Seek(0, SeekOrigin.Current);
            Console.Write("{0,8} ", pos.ToString("D8"));

            if(Indent * IndentSpaces > 0)
            {
                Console.Write(new string(Enumerable.Repeat(' ', Indent * IndentSpaces).ToArray()));
            }
            Console.Write("{0}: ", typeof(TType).Name);
            Console.WriteLine(message, args);
        }
    }
}
