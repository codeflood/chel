using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel.UnitTests.SampleCommands
{
    [Command("sample", "diffmod")]
    public class DuplicateSampleCommandDifferentModule : ICommand
    {
        public CommandResult Execute()
        {
            return new SuccessResult();
        }
    }
}