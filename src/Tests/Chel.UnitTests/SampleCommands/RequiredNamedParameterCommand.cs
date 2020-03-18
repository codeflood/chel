using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel.UnitTests.SampleCommands
{
    [Command("command")]
    [Description("A sample command with a required named parameter.")]
    public class RequiredNamedParameterCommand : ICommand
    {
        [NamedParameter("param")]
        [Required]
        public string NamedParameter { get; set; }

        public CommandResult Execute()
        {
            return new ValueResult(NamedParameter);
        }
    }
}