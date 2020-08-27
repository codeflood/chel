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
        private IPhraseDictionary _phraseDictionary;

        [NumberedParameter(1, "command")]
        [Description("The name of the command the display help for.")]
        public string CommandName { get; set; }

        public Help(ICommandRegistry commandRegistry, IPhraseDictionary phraseDictionary)
        {
            if(commandRegistry == null)
                throw new ArgumentNullException(nameof(commandRegistry));

            if(phraseDictionary == null)
                throw new ArgumentNullException(nameof(phraseDictionary));

            _commandRegistry = commandRegistry;
            _phraseDictionary = phraseDictionary;
        }

        public CommandResult Execute()
        {
            var cultureName = Thread.CurrentThread.CurrentCulture.Name;

            // todo: temporary implementation. Help should output a hash, once we have those in the type system.
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
            output.Append(_phraseDictionary.GetPhrase(Texts.PhraseKeys.AvailableCommands, cultureName));
            output.Append(":");
            output.Append(Environment.NewLine);

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

            output.Append(_phraseDictionary.GetPhrase(Texts.PhraseKeys.Usage, cultureName));
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
            output.AppendLine(command.GetDescription(cultureName));
            output.Append(Environment.NewLine);

            foreach(var numberedParameter in command.NumberedParameters)
            {
                var description = numberedParameter.GetDescription(cultureName);
                AppendParameterDetail(numberedParameter.PlaceholderText, description, numberedParameter.Required, cultureName, output);
            }

            foreach(var namedParameter in command.NamedParameters)
            {
                var description = namedParameter.Value.GetDescription(cultureName);
                var segment = $"-{namedParameter.Value.Name} <{namedParameter.Value.ValuePlaceholderText}>";
                AppendParameterDetail(segment, description, namedParameter.Value.Required, cultureName, output);
            }

            foreach(var flagParameter in command.FlagParameters)
            {
                var description = flagParameter.GetDescription(cultureName);
                AppendParameterDetail($"-{flagParameter.Name}", description, false, cultureName, output);
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

        private void AppendParameterDetail(string name, string description, bool required, string cultureName, StringBuilder output)
        {
            output.Append($"{name, -20}");

            if(required)
            {
                output.Append(_phraseDictionary.GetPhrase(Texts.PhraseKeys.Required, cultureName));
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