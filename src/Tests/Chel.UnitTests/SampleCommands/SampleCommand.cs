using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel.UnitTests.SampleCommands
{
    [Command("sample")]
    public class SampleCommand : ICommand
    {
        public CommandResult Execute()
        {
            return new SuccessResult();
        }
    }
}