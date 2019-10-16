using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel.UnitTests.SampleCommands
{
    [Command("  ")]
    public class InvalidCommandNameSampleCommand : ICommand
    {
        public CommandResult Execute()
        {
            return new SuccessResult();
        }
    }
}