using System;
using System.Text;
using Chel.Abstractions;
using Chel.Abstractions.Types;
using Chel.Abstractions.Variables;
using Chel.Exceptions;

namespace Chel
{
	internal class VariableReplacer : IVariableReplacer
    {
        public string ReplaceVariables(VariableCollection variables, ChelType input)
        {
            if(variables == null)
                throw new ArgumentNullException(nameof(variables));

            if(input == null)
                throw new ArgumentNullException(nameof(input));

            var output = new StringBuilder();

            ProcessParameter(input, output, variables);
            return output.ToString();
        }

        private void ProcessParameter(ChelType input, StringBuilder output, VariableCollection variables)
        {
            switch(input)
            {
                case SingleValue singleValue:
                    ProcessSingleValue(singleValue, output, variables);
                    break;

                case Literal literal:
                    output.Append(literal.Value);
                    break;

                case VariableReference variableReference:
                    ProcessVariableReference(variableReference, output, variables);
                    break;
            }
        }

        private void ProcessSingleValue(SingleValue input, StringBuilder output, VariableCollection variables)
        {
            foreach(var subvalue in input.Values)
            {
                ProcessParameter(subvalue, output, variables);
            }
        }

        private void ProcessVariableReference(VariableReference input, StringBuilder output, VariableCollection variables)
        {
            var variable = variables.Get(input.VariableName);

            if(variable == null)
                throw new UnsetVariableException(input.VariableName);

            output.Append(variable.Value);
        }
    }
}
