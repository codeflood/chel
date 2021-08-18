using System;
using System.Text;
using Chel.Abstractions;
using Chel.Abstractions.Parsing;
using Chel.Abstractions.Variables;
using Chel.Exceptions;

namespace Chel
{
	internal class VariableReplacer : IVariableReplacer
    {
        public string ReplaceVariables(VariableCollection variables, CommandParameter input)
        {
            if(variables == null)
                throw new ArgumentNullException(nameof(variables));

            if(input == null)
                throw new ArgumentNullException(nameof(input));

            var output = new StringBuilder();

            ProcessParameter(input, output, variables);
            return output.ToString();
        }

        private void ProcessParameter(CommandParameter parameter, StringBuilder output, VariableCollection variables)
        {
            switch(parameter)
            {
                case AggregateCommandParameter aggregateCommandParameter:
                    ProcessAggregateParameter(aggregateCommandParameter, output, variables);
                    break;

                case LiteralCommandParameter literalCommandParameter:
                    output.Append(literalCommandParameter.Value);
                    break;

                case VariableCommandParameter variableCommandParameter:
                    ProcessVariableParameter(variableCommandParameter, output, variables);
                    break;
            }
        }

        private void ProcessAggregateParameter(AggregateCommandParameter parameter, StringBuilder output, VariableCollection variables)
        {
            foreach(var subparameter in parameter.Parameters)
            {
                ProcessParameter(subparameter, output, variables);
            }
        }

        private void ProcessVariableParameter(VariableCommandParameter parameter, StringBuilder output, VariableCollection variables)
        {
            var variable = variables.Get(parameter.VariableName);

            if(variable == null)
                throw new UnsetVariableException(parameter.VariableName);

            output.Append(variable.Value);
        }
    }
}
