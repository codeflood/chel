using System;
using System.Linq;
using System.Text;
using System.Threading;
using Chel.Abstractions;
using Chel.Abstractions.Results;
using Chel.Abstractions.Types;

namespace Chel.Commands
{
    [Command("help")]
    [Description("Lists available commands and displays help for commands.")]
    public class Help : ICommand
    {
        private ICommandRegistry _commandRegistry;

        private string _executionCultureName = string.Empty;

        [NumberedParameter(1, "command")]
        [Description("The name of the command the display help for.")]
        public string CommandName { get; set; }

        public Help(ICommandRegistry commandRegistry)
        {
            _commandRegistry = commandRegistry ?? throw new ArgumentNullException(nameof(commandRegistry));
        }

        public CommandResult Execute()
        {
            // todo: Get culture from context. The command might be executed remotely and need a different culture.
            _executionCultureName = Thread.CurrentThread.CurrentCulture.Name;

            var output = new StringBuilder();

            if(string.IsNullOrEmpty(CommandName))
                ListCommands(output);
            else
            {
                var successful = DetailCommand(output);
                if(!successful)
                    return new FailureResult(ApplicationTextResolver.Instance.ResolveAndFormat(ApplicationTexts.CannotDisplayHelpUnknownCommnad, CommandName));
            }

            return new ValueResult(new Literal(output.ToString()));
        }

        private void ListCommands(StringBuilder output)
        {
            output.Append(ApplicationTextResolver.Instance.Resolve(ApplicationTexts.AvailableCommands));
            output.Append(":");
            output.Append(Environment.NewLine);

            foreach(var descriptor in _commandRegistry.GetAllRegistrations().OrderBy(x => x.CommandName))
            {
                output.Append($"{descriptor.CommandName, Constants.FirstColumnWidth}{descriptor.GetDescription(_executionCultureName)}");
                output.Append(Environment.NewLine);
            }

            output.Remove(output.Length - 1, 1);
        }

        private bool DetailCommand(StringBuilder output)
        {
            var command = _commandRegistry.Resolve(CommandName);
            if(command == null)
                return false;

            output.Append(ApplicationTextResolver.Instance.Resolve(ApplicationTexts.Usage));
            output.Append(": ");
            output.Append(command.CommandName);

            foreach(var numberedParameter in command.NumberedParameters)
            {
                output.Append(" ");
                AppendParameterUsage(numberedParameter.PlaceholderText, numberedParameter.Required, output);
            }

            foreach(var namedParameter in command.NamedParameters)
            {
                output.Append(" ");
                AppendNamedParameterUsage(namedParameter.Value.Name, namedParameter.Value.ValuePlaceholderText, namedParameter.Value.Required, output);
            }

            foreach(var flagParameter in command.FlagParameters)
            {
                output.Append(" ");
                AppendParameterUsage($"-{flagParameter.Name}", false, output);
            }

            output.Append(Environment.NewLine);
            output.AppendLine(command.GetDescription(_executionCultureName));
            output.Append(Environment.NewLine);

            foreach(var numberedParameter in command.NumberedParameters)
            {
                var description = numberedParameter.GetDescription(_executionCultureName);
                AppendParameterDetail(numberedParameter.PlaceholderText, description, numberedParameter.Required, output);
            }

            foreach(var namedParameter in command.NamedParameters)
            {
                var description = namedParameter.Value.GetDescription(_executionCultureName);
                var segment = $"-{namedParameter.Value.Name} <{namedParameter.Value.ValuePlaceholderText}>";
                AppendParameterDetail(segment, description, namedParameter.Value.Required, output);
            }

            foreach(var flagParameter in command.FlagParameters)
            {
                var description = flagParameter.GetDescription(_executionCultureName);
                AppendParameterDetail($"-{flagParameter.Name}", description, false, output);
            }

            return true;
        }

        private void AppendParameterUsage(string name, bool required, StringBuilder output)
        {
            if(!required)
                output.Append("[");

            output.Append(name);

            if(!required)
                output.Append("]");
        }

        private void AppendNamedParameterUsage(string name, string valuePlaceholder, bool required, StringBuilder output)
        {
            var segment = $"-{name} <{valuePlaceholder}>";
            AppendParameterUsage(segment, required, output);
        }

        private void AppendParameterDetail(string name, string description, bool required, StringBuilder output)
        {
            output.Append($"{name, Constants.FirstColumnWidth}");

            if(required)
            {
                output.Append(ApplicationTextResolver.Instance.Resolve(ApplicationTexts.Required));
                output.Append(". ");
            }
                
            output.AppendLine(description);
        }

        /*
        Possible alternative output; group optional parameters together. Example :=

        usage: command reqNum -reqNam <value> [options]
        The command description

        reqNum              A numbered parameter
        -reqNam <value>     A named parameter

        options:
        -optionalNamed <value> An optional named parameter
        -f1                 A flag parameter
        */
    }
}