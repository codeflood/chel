using System.Collections.Generic;
using System.Linq;
using Chel.Abstractions;

namespace Chel;

/// <summary>
/// A script provider that reads scripts from multiple other script providers.
/// </summary>
public class ScriptProviderCollection : IScriptProvider
{
    private readonly IEnumerable<IScriptProvider> _providers;

    public ScriptProviderCollection(IEnumerable<IScriptProvider> providers)
    {
        _providers = providers ?? new List<IScriptProvider>();
    }

    public IList<ExecutionTargetIdentifier> GetScriptNames()
    {
        var scriptNames = new HashSet<ExecutionTargetIdentifier>();

        foreach(var provider in _providers)
        {
            foreach(var target in provider.GetScriptNames())
            {
                scriptNames.Add(target);
            }
        }

        return scriptNames.ToList();
    }

    public string? GetScriptSource(string? module, string name)
    {
        foreach(var provider in _providers.Reverse())
        {
            var source = provider.GetScriptSource(module, name);
            if(source != null)
                return source;
        }

        return null;
    }
}