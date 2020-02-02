using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel.Commands
{
    [Command("echo")]
    [Description("Output the supplied parameters.")]
    public class Echo : ICommand
    {
        [NumberedParameter(1, "message")]
        [Description("The message to output.")]
        public string Message { get; set; }

        public CommandResult Execute()
        {
            return new ValueResult(Message ?? string.Empty);
        }
    }
}