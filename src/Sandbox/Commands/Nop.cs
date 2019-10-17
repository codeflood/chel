using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Sandbox.Commands
{
    [Command("nop")]
    [Description("Do nothing.")]
    public class Nop : ICommand
    {
        public CommandResult Execute()
        {
            return new SuccessResult();
        }
    }
}