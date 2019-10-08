using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel.UnitTests.SampleCommands
{
    [Command("sample", "A duplicate sample command.")]
    public class DuplicateSampleCommand : ICommand
    {
        public CommandResult Execute()
        {
            return new SuccessResult();
        }
    }
}