using System;
using System.Linq;
using System.Text;
using System.Threading;
using Chel.Abstractions;
using Chel.Abstractions.Parsing;
using Chel.Abstractions.Results;
using Chel.Abstractions.Types;

namespace Chel.Commands
{
    [Command("help")]
    [Description("Lists available commands and displays help for commands.")]
    public class Help : ICommand
    {
        private readonly ICommandRegistry _commandRegistry;
        private readonly IExecutionTargetIdentifierParser _executionTargetIdentifierParser;

        private string _executionCultureName = string.Empty;

        [NumberedParameter(1, "command")]
        [Description("The identifier of the command or module the display help for.")]
        public string? CommandIdentifier { get; set; }

        public Help(ICommandRegistry commandRegistry, IExecutionTargetIdentifierParser executionTargetIdentifierParser)
        {
            _commandRegistry = commandRegistry ?? throw new ArgumentNullException(nameof(commandRegistry));
            _executionTargetIdentifierParser = executionTargetIdentifierParser ?? throw new ArgumentNullException(nameof(executionTargetIdentifierParser));
        }

        public CommandResult Execute()
        {
            // todo: Get culture from context. The command might be executed remotely and need a different culture.
            _executionCultureName = Thread.CurrentThread.CurrentCulture.Name;

            var output = new StringBuilder();

            var identifier = _executionTargetIdentifierParser.Parse(CommandIdentifier ?? string.Empty);

            if(!string.IsNullOrEmpty(identifier.Name))
            {
                var successful = DetailCommand(output, identifier);
                if(!successful)
                    return new FailureResult(ApplicationTextResolver.Instance.ResolveAndFormat(ApplicationTexts.CannotDisplayHelpUnknownCommand, identifier));
            }
            else if(!string.IsNullOrEmpty(identifier.Module))
            {
                var successful = ListCommands(output, identifier.Module);

                if(!successful)
                    return new FailureResult(ApplicationTextResolver.Instance.ResolveAndFormat(ApplicationTexts.CannotDisplayHelpUnknownModule, identifier.Module ?? string.Empty));

                output.Append(Environment.NewLine);
                output.Append(ApplicationTextResolver.Instance.Resolve(ApplicationTexts.SpecificCommandHelp));
            }
            else
            {
                ListCommands(output, null);
                output.Append(Environment.NewLine);
                ListModules(output);
                output.Append(Environment.NewLine);
                output.Append(ApplicationTextResolver.Instance.Resolve(ApplicationTexts.SpecificCommandHelp));
                output.Append(Environment.NewLine);
                output.Append(ApplicationTextResolver.Instance.Resolve(ApplicationTexts.SpecificModuleHelp));
            }

            return new ValueResult(new Literal(output.ToString()));
        }

        private bool ListCommands(StringBuilder output, string? moduleName)
        {
            output.Append(ApplicationTextResolver.Instance.Resolve(ApplicationTexts.AvailableCommands));
            output.Append(":");
            output.Append(Environment.NewLine);

            var descriptors = _commandRegistry.GetAllRegistrations();

            if(moduleName == null)
                descriptors = descriptors.Where(x => x.CommandIdentifier.Module == null);
            else
            {
                // todo: It feels odd handling the case sensitivity here
                descriptors = descriptors.Where(x => x.CommandIdentifier.Module != null && x.CommandIdentifier.Module.Equals(moduleName, StringComparison.OrdinalIgnoreCase));
                if(!descriptors.Any())
                    return false;
            }

            foreach(var descriptor in descriptors.OrderBy(x => x.CommandIdentifier.Name))
            {
                output.Append($"{descriptor.CommandIdentifier.Name, Constants.FirstColumnWidth}{descriptor.GetDescription(_executionCultureName)}");
                output.Append(Environment.NewLine);
            }

            return true;
        }

        private void ListModules(StringBuilder output)
        {
            output.Append(ApplicationTextResolver.Instance.Resolve(ApplicationTexts.AvailableModules));
            output.Append(":");
            output.Append(Environment.NewLine);

            var descriptors = _commandRegistry.GetAllRegistrations().Where(x => x.CommandIdentifier.Module != null).OrderBy(x => x.CommandIdentifier.Name);

            foreach(var descriptorGroup in descriptors.GroupBy(x => x.CommandIdentifier.Module).OrderBy(x => (x.Key ?? string.Empty).ToLower()))
            {
                output.Append(descriptorGroup.Key);
                output.Append(", ");
            }

            output.Remove(output.Length - 2, 2);
            output.Append(Environment.NewLine);
        }

        private bool DetailCommand(StringBuilder output, ExecutionTargetIdentifier commandIdentifier)
        {
            var command = _commandRegistry.Resolve(commandIdentifier);
            if(command == null)
                return false;

            output.Append(ApplicationTextResolver.Instance.Resolve(ApplicationTexts.Usage));
            output.Append(": ");
            output.Append(command.CommandIdentifier);

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

        private void AppendParameterDetail(string name, string? description, bool required, StringBuilder output)
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