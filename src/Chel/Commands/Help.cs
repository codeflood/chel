using System;
using System.Text;
using System.Threading;
using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel.Commands
{
    [Command("help")]
    [Description("Lists available commands and displays help for commands.")]
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
            var cultureName = Thread.CurrentThread.CurrentCulture.Name;

            var output = new StringBuilder();
            output.Append($"{Texts.AvailableCommands}:{Environment.NewLine}");

            foreach(var descriptor in _commandRegistry.GetAllRegistrations())
            {
                output.Append($"{descriptor.CommandName, -25}{descriptor.GetDescription(cultureName)}{Environment.NewLine}");
            }

            return new ValueResult(output.ToString());
        }
    }
}