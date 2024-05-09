using System.Collections.Generic;

namespace Chel.Abstractions;

/// <summary>
/// Defines a provider of scripts.
/// </summary>
public interface IScriptProvider
{
    /// <summary>
    /// Get the source of a script.
    /// </summary>
    /// <param name="module">The optional name of the module the script belongs to.</param>
    /// <param name="name">The name of the script to get the source of.</param>
    /// <returns>The script source if the script exists.</returns>
    string? GetScriptSource(string? module, string name);

    /// <summary>
    /// Gets the names of all the scripts the provider has.
    /// </summary>
    /// <returns>The names of all the scripts.</returns>
    IList<ExecutionTargetIdentifier> GetScriptNames();
}
