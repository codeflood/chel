using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel.UnitTests.SampleCommands
{
    [Command("sample")]
    public class DuplicateSampleCommand : ICommand
    {
        public CommandResult Execute()
        {
            return SuccessResult.Instance;
        }
    }
}