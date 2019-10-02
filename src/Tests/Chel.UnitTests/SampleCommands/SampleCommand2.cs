using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel.UnitTests.SampleCommands
{
    [Command("sample2")]
    public class SampleCommand2 : ICommand
    {
        public CommandResult Execute()
        {
            return new Success();
        }
    }
}