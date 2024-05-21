using System;
using System.Collections.Generic;
using Chel.Abstractions;
using Chel.Abstractions.Results;
using Chel.Abstractions.Types;

namespace Chel.Commands;

[Command("scripts")]
[Description("Lists available scritps.")]
public class Scripts : ICommand
{
    private readonly IScriptProvider _scriptProvider;

    public Scripts(IScriptProvider scriptProvider)
    {
        _scriptProvider = scriptProvider ?? throw new ArgumentNullException(nameof(scriptProvider));
    }

    public CommandResult Execute()
    {
        var names = new List<ChelType>();

        foreach(var name in _scriptProvider.GetScriptNames())
        {
            var literal = new Literal(name.ToString());
            names.Add(literal);
        }

        return new ValueResult(new List(names));
    }
}
