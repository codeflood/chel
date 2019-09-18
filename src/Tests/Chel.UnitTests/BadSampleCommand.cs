using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel.UnitTests
{
    // Missing Command attribute.
    public class BadSampleCommand : ICommand
    {
        public CommandResult Execute()
        {
            return new Success();
        }
    }
}