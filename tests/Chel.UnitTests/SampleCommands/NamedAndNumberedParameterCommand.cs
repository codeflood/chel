using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel.UnitTests.SampleCommands
{
    [Command("☕")]
    public class NamedAndNumberedParameterCommand : ICommand
    {
        [NamedParameter("named", "named")]
        public string NamedParameter { get; set; }

        [NumberedParameter(1, "num")]
        public string NumberedParameter { get; set; }

        public CommandResult Execute()
        {
            var value = (NamedParameter ?? string.Empty) + " " + (NumberedParameter ?? string.Empty);
            return new ValueResult(value);
        }
    }
}