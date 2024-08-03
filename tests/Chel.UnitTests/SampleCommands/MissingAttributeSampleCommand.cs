using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel.UnitTests.SampleCommands
{
    // Missing Command attribute.
    public class MissingAttributeSampleCommand : ICommand
    {
        public CommandResult Execute()
        {
            return SuccessResult.Instance;
        }
    }
}