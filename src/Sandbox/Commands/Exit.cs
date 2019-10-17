using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Sandbox.Commands
{
    [Command("exit")]
    [Description("Exit the sandbox.")]
    public class Exit : ICommand
    {
        public CommandResult Execute()
        {
            return new ExitResult();
        }
    }
}