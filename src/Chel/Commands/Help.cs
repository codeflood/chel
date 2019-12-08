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

        [NumberedParameter(1, "command")]
        [Description("The name of the command the display help for.")]
        public string CommandName { get; set; }

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

            if(string.IsNullOrEmpty(CommandName))
                ListCommands(cultureName, output);
            else
            {
                var successful = DetailCommand(cultureName, output);
                if(!successful)
                    // todo: How to handle the line number; the command shouldn't know it
                    return new FailureResult(1, new[]{ string.Format(Texts.CannotDisplayHelpUnknownCommnad, CommandName) });
            }

            return new ValueResult(output.ToString());
        }

        private void ListCommands(string cultureName, StringBuilder output)
        {
            output.Append($"{Texts.AvailableCommands}:{Environment.NewLine}");

            foreach(var descriptor in _commandRegistry.GetAllRegistrations())
            {
                output.Append($"{descriptor.CommandName, -25}{descriptor.GetDescription(cultureName)}{Environment.NewLine}");
            }
        }

        private bool DetailCommand(string cultureName, StringBuilder output)
        {
            var command = _commandRegistry.Resolve(CommandName);
            if(command == null)
                return false;

            output.Append(command.CommandName);

            foreach(var numberedParameter in command.NumberedParameters)
            {
                output.Append(" ");
                output.Append(numberedParameter.PlaceholderText);
            }

            output.Append(Environment.NewLine);
            output.AppendLine(command.GetDescription(cultureName));
            output.Append(Environment.NewLine);

            foreach(var numberedParameter in command.NumberedParameters)
            {
                output.Append($"{numberedParameter.PlaceholderText, -20}");
                output.AppendLine(numberedParameter.GetDescription(cultureName));
            }

            return true;
        }
    }
}