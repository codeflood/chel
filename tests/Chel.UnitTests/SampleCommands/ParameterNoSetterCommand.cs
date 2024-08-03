using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel.UnitTests.SampleCommands
{
    [Command("command")]
    public class ParameterNoSetterCommand : ICommand
    {
        [NumberedParameter(1, "noset")]
        public string NoSet { get; }

        public CommandResult Execute()
        {
            return SuccessResult.Instance;
        }
    }
}