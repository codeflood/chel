using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel.UnitTests.SampleCommands
{
    [Command("cmd", "DIFFMOD")]
    public class AnotherCommandDifferentModuleUppercase : ICommand
    {
        public CommandResult Execute()
        {
            return SuccessResult.Instance;
        }
    }
}