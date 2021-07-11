using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel.UnitTests.SampleCommands
{
    [Command("command")]
    public class RequiredFlagParameterCommand : ICommand
    {
        [FlagParameter("f")]
        [Required]
        public bool Flag { get; set; }

        public CommandResult Execute()
        {
            return new ValueResult(Flag.ToString());
        }
    }
}