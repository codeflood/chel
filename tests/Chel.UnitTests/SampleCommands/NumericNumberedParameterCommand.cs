using System.Text;
using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel.UnitTests.SampleCommands
{
    [Command("command")]
    public class NumericNumberedParameterCommand : ICommand
    {
        [NumberedParameter(1, "num")]
        public int NumberedParameter { get; set; }

        public CommandResult Execute()
        {
            return new ValueResult(NumberedParameter.ToString());
        }
    }
}