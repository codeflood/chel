using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel.UnitTests
{
    [Command("sample")]
    public class SampleCommand : ICommand
    {
        public CommandResult Execute()
        {
            return new Success();
        }
    }
}