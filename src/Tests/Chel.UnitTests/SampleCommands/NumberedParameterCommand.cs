using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel.UnitTests.SampleCommands
{
    [Command("num")]
    public class NumberedParameterCommand : ICommand
    {
        [NumberedParameter(1)]
        public string NumberedParameter1 { get; set; }

        [NumberedParameter(2)]
        public string NumberedParameter2 { get; set; }

        public NumberedParameterCommand()
        {
            NumberedParameter1 = null;
            NumberedParameter2 = null;
        }

        public CommandResult Execute()
        {
            var value = (NumberedParameter1 ?? string.Empty) + " " + (NumberedParameter2 ?? string.Empty);
            return new ValueResult(value);
        }
    }
}