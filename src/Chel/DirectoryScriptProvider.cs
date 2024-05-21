using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chel.Abstractions;

namespace Chel;

/// <summary>
/// A script provider that reads script files from a local directory.
/// </summary>
public class DirectoryScriptProvider : IScriptProvider
{
    public const string ScriptFileExtension = "chel";

    private readonly string _path;

    public DirectoryScriptProvider(string path)
    {
        _path = path ?? throw new ArgumentNullException(nameof(path));
    }

    public IList<ExecutionTargetIdentifier> GetScriptNames()
    {
        var results = new List<ExecutionTargetIdentifier>();

        if(Directory.Exists(_path))
        {
            AddScriptNamesFromPath(_path, null, results);
            var modulePaths = Directory.EnumerateDirectories(_path);
            foreach(var modulePath in modulePaths)
            {
                // The leaf here is a directory
                var moduleName = Path.GetFileName(modulePath).ToLowerInvariant();
                AddScriptNamesFromPath(modulePath, moduleName, results);
            }
        }

        return results;
    }

    public string? GetScriptSource(string? module, string name)
    {
        // Casing might be different. Search for a case-insensitive match.
        var path = _path;

        if(module != null)
        {
            var directories = Directory.EnumerateDirectories(_path);
            foreach(var directory in directories)
            {
                if(string.Compare(directory, module, true) == 0)
                {
                    path = Path.Combine(_path, directory);
                    break;
                }
            }

            return null;
        }

        var files = Directory.EnumerateFiles(path, $"*.{ScriptFileExtension}");
        foreach(var file in files.OrderBy(x => x))
        {
            var filename = Path.GetFileNameWithoutExtension(file);
            if(string.Compare(filename, name, true) == 0)
                return File.ReadAllText(file);
        }

        return null;
    }

    private void AddScriptNamesFromPath(string path, string module, List<ExecutionTargetIdentifier> results)
    {
        var names = new HashSet<ExecutionTargetIdentifier>();
        var files = Directory.EnumerateFiles(path, $"*.{ScriptFileExtension}");

        foreach(var file in files)
        {
            var name = Path.GetFileNameWithoutExtension(file).ToLowerInvariant();
            names.Add(new ExecutionTargetIdentifier(module, name));
        }

        results.AddRange(names);
    }
}
