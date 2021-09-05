using System.Text;
using Chel.Abstractions;
using Chel.Abstractions.Results;
using Chel.Abstractions.Types;

namespace Chel.UnitTests.SampleCommands
{
    [Command("command")]
    public class NumericNumberedParameterCommand : ICommand
    {
        [NumberedParameter(1, "num")]
        public int NumberedParameter { get; set; }

        public CommandResult Execute()
        {
            return new ValueResult(new Literal(NumberedParameter.ToString()));
        }
    }
}