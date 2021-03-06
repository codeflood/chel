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

        [FlagParameter("upper")]
        [Description("Output the message in uppercase.")]
        public bool Uppercase { get; set; }

        public CommandResult Execute()
        {
            var message = Message ?? string.Empty;

            if(Uppercase)
                message = message.ToUpper();

            return new ValueResult(message);
        }
    }
}