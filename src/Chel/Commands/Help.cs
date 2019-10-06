using System;
using System.Text;
using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel.Commands
{
    [Command("help")]
    public class Help : ICommand
    {
        private ICommandRegistry _commandRegistry;

        public Help(ICommandRegistry commandRegistry)
        {
            if(commandRegistry == null)
                throw new ArgumentNullException(nameof(commandRegistry));

            _commandRegistry = commandRegistry;
        }

        public CommandResult Execute()
        {
            // todo: temporary implementation.
            var output = new StringBuilder();
            output.Append($"{Texts.AvailableCommands}:{Environment.NewLine}");

            foreach(var descriptor in _commandRegistry.GetAllRegistrations())
            {
                output.Append($"{descriptor.CommandName}{Environment.NewLine}");
            }

            return new ValueResult(output.ToString());
        }
    }
}