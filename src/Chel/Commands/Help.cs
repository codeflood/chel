using System;
using System.Collections.Generic;
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
        private IPhraseDictionary _phraseDictionary;

        private string _executionCultureName = string.Empty;

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
            _executionCultureName = Thread.CurrentThread.CurrentCulture.Name;

            // todo: temporary implementation. Help should output a hash, once we have those in the type system.
            ChelType output = null;

            if(string.IsNullOrEmpty(CommandName))
                output = ListCommands();
            else
            {
                var detailOutput = new StringBuilder();
                var successful = DetailCommand(detailOutput);
                if(!successful)
                    // todo: How to handle the line number; the command shouldn't know it
                    return new FailureResult(1, new[]{ string.Format(Texts.CannotDisplayHelpUnknownCommnad, CommandName) });

                output = new Literal(detailOutput.ToString());
            }

            return new ValueResult(output);
        }

        private List ListCommands()
        {
            var lines = new List<Literal>();

            lines.Add(new Literal(_phraseDictionary.GetPhrase(Texts.PhraseKeys.AvailableCommands, _executionCultureName) + ":"));

            foreach(var descriptor in _commandRegistry.GetAllRegistrations())
            {
                lines.Add(new Literal($"{descriptor.CommandName, Constants.FirstColumnWidth}{descriptor.GetDescription(_executionCultureName)}"));
            }

            return new List(lines);
        }

        private bool DetailCommand(StringBuilder output)
        {
            var command = _commandRegistry.Resolve(CommandName);
            if(command == null)
                return false;

            output.Append(_phraseDictionary.GetPhrase(Texts.PhraseKeys.Usage, _executionCultureName));
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
                AppendParameterDetail(numberedParameter.PlaceholderText, description, numberedParameter.Required, _executionCultureName, output);
            }

            foreach(var namedParameter in command.NamedParameters)
            {
                var description = namedParameter.Value.GetDescription(_executionCultureName);
                var segment = $"-{namedParameter.Value.Name} <{namedParameter.Value.ValuePlaceholderText}>";
                AppendParameterDetail(segment, description, namedParameter.Value.Required, _executionCultureName, output);
            }

            foreach(var flagParameter in command.FlagParameters)
            {
                var description = flagParameter.GetDescription(_executionCultureName);
                AppendParameterDetail($"-{flagParameter.Name}", description, false, _executionCultureName, output);
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
            output.Append($"{name, Constants.FirstColumnWidth}");

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