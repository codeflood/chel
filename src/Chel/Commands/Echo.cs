using Chel.Abstractions;
using Chel.Abstractions.Results;
using Chel.Abstractions.Types;

namespace Chel.Commands
{
    [Command("echo")]
    [Description("Output the supplied parameter.")]
    public class Echo : ICommand
    {
        [NumberedParameter(1, "message")]
        [Description("The message to output.")]
        public ChelType? Message { get; set; }

        [FlagParameter("upper")]
        [Description("Output the message in uppercase.")]
        public bool Uppercase { get; set; }

        public CommandResult Execute()
        {
            var message = Message?.ToString() ?? string.Empty;

            if(Uppercase)
                message = message.ToUpper();

            return new ValueResult(new Literal(message));
        }
    }
}