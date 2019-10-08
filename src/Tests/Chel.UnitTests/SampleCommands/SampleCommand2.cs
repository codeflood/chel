using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel.UnitTests.SampleCommands
{
    [Command("sample2", "Another sample command which does nothing.")]
    public class SampleCommand2 : ICommand
    {
        public CommandResult Execute()
        {
            return new SuccessResult();
        }
    }
}