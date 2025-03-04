﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Legends.Content.Auto;

public class Main : IDisposable
{
    private readonly FileSystemWatcher _watcher;    
    private readonly Dictionary<string, string> _templates;
    private readonly List<string> _ignore;

    private string _header;

    private bool _debug = false;

    public void DebugMessage(string message, params object[] args)
    {
        if(_debug)
            Console.WriteLine(message, args);
    }
    
    public static string GetExecutingDirectoryName()
    {
        var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
        return new FileInfo(location.AbsolutePath).Directory.FullName;
    }

    public void LoadAssetTemplates(string templateDir)
    {
        foreach(var file in Directory.GetFiles(templateDir))
        {
            if(Path.GetFileName(file) == ".ignore")
            {
                DebugMessage("Found Ignore File");
                _ignore.AddRange(File.ReadAllLines(file).ToList());                
                DebugMessage("Ignoring Extensions:\n{0}\n", string.Join('\n', _ignore));
                continue;
            }
            if(string.Compare(Path.GetFileName(file), "template.mgcb", true) == 0)
            {
                DebugMessage("Found Header File");
                _header = File.ReadAllText(file);
                DebugMessage("{0}\n", _header);
                continue;
            }
            
            DebugMessage("Found Template .{0}", Path.GetFileNameWithoutExtension(file));
            _templates.Add("." + Path.GetFileNameWithoutExtension(file), File.ReadAllText(file));            
            DebugMessage("{0}\n", _templates["." + Path.GetFileNameWithoutExtension(file)]);
        }
    }

    public string GetAssetTemplate(string fileName)
    {
        if(_templates.TryGetValue(Path.GetExtension(fileName), out string result))
        {
            DebugMessage("Found Template for {0}", fileName);
            return result;
        }
        throw new KeyNotFoundException(string.Format("No template found for asset: {0}", fileName));
    }

    public void BuildSingle(string fileName, bool force = false)
    {
        Console.WriteLine("Building {0}.", fileName);
        string content = string.Format(GetAssetTemplate(fileName), Path.GetRelativePath(_watcher.Path, fileName));
        string contentFile = Path.GetFileName(Path.ChangeExtension(Path.GetRandomFileName(), "mgcb"));

        DebugMessage("Single Asset Template [{0}]\n{1}", contentFile, content);

        File.WriteAllText(Path.Combine(_watcher.Path, contentFile), _header + "\n\n" + content);
        try {
            ExecuteCommand("dotnet", "mgcb", contentFile,force?"/c": "/I");
        }
        finally {
            File.Delete(Path.Combine(_watcher.Path, contentFile));
        }
    }

    public void BuildAll(bool force = false)
    {
         ExecuteCommand("dotnet", "mgcb", contentDb,force?"/c": "");
    }

    public void ExecuteCommand(string command, params string[] flags)
    {
        Console.WriteLine("[exec] {0} {1}", command, string.Join(' ', flags.Select(n => n.ToString())));
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = string.Join(' ', flags.Select(n => n.ToString())),
                WorkingDirectory = _watcher.Path,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        List<string> results = new();

        // hookup the eventhandlers to capture the data that is received
        process.OutputDataReceived += (sender, args) => { results.Add(args.Data); DebugMessage("{0}", args.Data); };
        process.ErrorDataReceived += (sender, args) => { results.Add(args.Data); Console.WriteLine("{0}", args.Data); };
       
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();
        var result = string.Join('\n', results.Skip(results.Count - 5).Where(n => !string.IsNullOrEmpty(n)));
        File.WriteAllText("auto-output.log", string.Join('\n', results));
        if(result.IndexOf("0 failed.") > 0)
        {
            Console.WriteLine("{0}", result);
            Console.WriteLine();     
        }
        else
        {
            Console.WriteLine(string.Join('\n', results));
            Console.WriteLine();     
        }
    }

    public void BuildContentDb(string contentFile)
    {
        Console.WriteLine("BuildContentDb for {0}/{1}", Path.GetFullPath(_watcher.Path), contentFile);
        string[] results = Directory.GetFiles(
            Path.GetFullPath(_watcher.Path), 
            "*.*", 
            SearchOption.AllDirectories).Where(n => _templates.ContainsKey(Path.GetExtension(n))).ToArray();

        StringBuilder sb = new StringBuilder();
        sb.AppendLine(_header);
        sb.AppendLine();

        foreach(var item in results)
        {
            if(!Ignore(item))
            {
                sb.AppendFormat(GetAssetTemplate(item), Path.GetRelativePath(_watcher.Path, item));
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine();
            }
        }
        
        File.WriteAllText(Path.Combine(_watcher.Path, contentFile), sb.ToString());
    }

    public bool Ignore(string file, bool fileDeleted = false)
    {     
        if(!fileDeleted)
        {
            if(!File.Exists(file)) return true;
            if(File.GetAttributes(file).HasFlag(FileAttributes.Directory)) return true;
        }
        if(_ignore.Any(n => 0 == string.Compare(Path.GetExtension(file), n)))
        {
            //DebugMessage("Ignoring file {0}", file);
            return true;
        }
        
        
       // DebugMessage("Not Ignoring file {0}", file);
        
        return false;
    }

    public bool Changed(string file)
    {
        if(_changed.TryGetValue(file, out DateTime changeTime))
        {
            if(System.IO.File.GetLastWriteTime(file) <= changeTime)
            {
                return false;
            }
        }

        _changed[file] = File.GetLastWriteTime(file);
        return true;
    }

    private Dictionary<string, DateTime> _changed = new();
    string contentDb = @"content.mgcb";        
    string templatePath = @"Templates";     
    string contentPath = @"../../../../Legends.App/Content";
    public Main()
    {   

        

        _ignore = new();
        _templates = new();

        LoadAssetTemplates(templatePath);

        _watcher = new FileSystemWatcher(Path.Combine(GetExecutingDirectoryName(), contentPath))
        {
            IncludeSubdirectories = true,
            EnableRaisingEvents = true
        };

        DebugMessage("Starting Watcher on {0}", _watcher.Path);

        _watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime;

        _watcher.Changed += (sender, args) => { 
            DebugMessage("Changed Event {0} {1}", args.FullPath, args.ChangeType);
            if(!Ignore(args.FullPath) && Changed(args.FullPath))
            {   
                BuildSingle(args.FullPath);
            }
        };

        _watcher.Created += (sender, args) => { 
            DebugMessage("Created Event {0}", args.FullPath);
            if(!Ignore(args.FullPath) && Changed(args.FullPath))
            {
                BuildContentDb(contentDb);
                BuildSingle(args.FullPath);
            }
        };

        _watcher.Deleted += (sender, args) => { 
            DebugMessage("Deleted Event {0}", args.FullPath);
            _changed.Remove(args.FullPath);
            if(!Ignore(args.FullPath, true))
            {
                BuildContentDb(contentDb);
            }
        };

        _watcher.Renamed += (sender, args) => { 
            DebugMessage("Renamed Event {0}", args.FullPath);
            _changed.Remove(args.OldFullPath);
            if(!Ignore(args.FullPath, true))
            {
                BuildContentDb(contentDb);
                BuildSingle(args.FullPath);
            }  
        };
    }

    public void Run()
    {


        while(true) {
            var keyInfo = Console.ReadKey(true);

            switch(keyInfo.Key)
            {
                case ConsoleKey.Escape: Environment.Exit(0); break;
                case ConsoleKey.Enter: 
                    BuildContentDb(contentDb); 
                    BuildAll(keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift));
                    break;
            }            
        }
    }

    public void Dispose()
    {

    }
}