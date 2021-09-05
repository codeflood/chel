using Chel.Abstractions;
using Chel.Abstractions.Results;
using Chel.Abstractions.Types;

namespace Chel.UnitTests.SampleCommands
{
    [Command("num")]
    [Description("A sample command with numbered parameters.")]
    public class NumberedParameterCommand : ICommand
    {
        [NumberedParameter(1, "param1")]
        [Description("The first parameter")]
        public string NumberedParameter1 { get; set; }

        [NumberedParameter(2, "param2")]
        [Description("The second parameter")]
        public string NumberedParameter2 { get; set; }

        public NumberedParameterCommand()
        {
            NumberedParameter1 = null;
            NumberedParameter2 = null;
        }

        public CommandResult Execute()
        {
            var value = (NumberedParameter1 ?? string.Empty) + " " + (NumberedParameter2 ?? string.Empty);
            return new ValueResult(new Literal(value));
        }
    }
}