using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Sandbox
{
    [Command("nop", "Do nothing.")]
    public class Nop : ICommand
    {
        public CommandResult Execute()
        {
            return new SuccessResult();
        }
    }
}