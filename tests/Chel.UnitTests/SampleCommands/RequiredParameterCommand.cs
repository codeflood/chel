using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel.UnitTests.SampleCommands
{
    [Command("command")]
    [Description("A sample command with a required parameter.")]
    public class RequiredParameterCommand : ICommand
    {
        [NumberedParameter(1, "param")]
        [Description("The first parameter")]
        [Required]
        public string NumberedParameter { get; set; }

        public CommandResult Execute()
        {
            return new ValueResult(NumberedParameter);
        }
    }
}