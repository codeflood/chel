using Chel.Abstractions;
using Chel.Abstractions.Results;
using Chel.Sandbox.Results;

namespace Chel.Sandbox.Commands
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