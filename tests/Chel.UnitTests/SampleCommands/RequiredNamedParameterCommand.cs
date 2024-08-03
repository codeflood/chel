using Chel.Abstractions;
using Chel.Abstractions.Results;
using Chel.Abstractions.Types;

namespace Chel.UnitTests.SampleCommands
{
    [Command("command")]
    [Description("A sample command with a required named parameter.")]
    public class RequiredNamedParameterCommand : ICommand
    {
        [NamedParameter("param", "value")]
        [Description("A required parameter.")]
        [Required]
        public string? NamedParameter { get; set; }

        public CommandResult Execute()
        {
            return new ValueResult(new Literal(NamedParameter ?? string.Empty));
        }
    }
}