using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Sandbox
{
    [Command("nop")]
    public class Nop : ICommand
    {
        public CommandResult Execute()
        {
            return new Success();
        }
    }
}