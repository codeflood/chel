using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Sandbox
{
    [Command("hi")]
    public class Greeter : ICommand
    {
        public CommandResult Execute()
        {
            return new Success();
        }
    }
}