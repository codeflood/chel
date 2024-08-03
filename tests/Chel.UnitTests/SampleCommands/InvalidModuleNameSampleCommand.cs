using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel.UnitTests.SampleCommands
{
    [Command("cmd", "  ")]
    public class InvalidModuleNameSampleCommand : ICommand
    {
        public CommandResult Execute()
        {
            return SuccessResult.Instance;
        }
    }
}