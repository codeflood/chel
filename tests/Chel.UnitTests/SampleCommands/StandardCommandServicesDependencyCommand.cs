using System;
using Chel.Abstractions;
using Chel.Abstractions.Parsing;
using Chel.Abstractions.Results;
using Chel.UnitTests.SampleObjects;

namespace Chel.UnitTests.SampleCommands;

[Command("cmd")]
public class StandardCommandServicesDependencyCommand : ICommand
{
    private readonly ICommandRegistry _commandRegistry = null;
    private readonly INameValidator _nameValidator = null;
    private readonly IExecutionTargetIdentifierParser _executionTargetIdentifierParser = null;
    private readonly IScriptProvider _scriptProvider = null;

    public StandardCommandServicesDependencyCommand(
        ICommandRegistry commandRegistry,
        INameValidator nameValidator,
        IExecutionTargetIdentifierParser executionTargetIdentifierParser,
        IScriptProvider scriptProvider
    )
    {
        _commandRegistry = commandRegistry ?? throw new ArgumentNullException(nameof(commandRegistry));
        _nameValidator = nameValidator ?? throw new ArgumentNullException(nameof(nameValidator));
        _executionTargetIdentifierParser = executionTargetIdentifierParser ?? throw new ArgumentNullException(nameof(executionTargetIdentifierParser));
        _scriptProvider = scriptProvider ?? throw new ArgumentNullException(nameof(scriptProvider));
    }

    public CommandResult Execute()
    {
        return new SuccessResult();
    }
}
