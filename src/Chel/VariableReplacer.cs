using System;
using System.Collections.Generic;
using Chel.Abstractions;
using Chel.Abstractions.Types;
using Chel.Abstractions.Variables;
using Chel.Exceptions;

namespace Chel
{
	internal class VariableReplacer : IVariableReplacer
    {
        public ChelType ReplaceVariables(VariableCollection variables, ChelType input)
        {
            if(variables == null)
                throw new ArgumentNullException(nameof(variables));

            if(input == null)
                throw new ArgumentNullException(nameof(input));

            return ProcessParameter(input, variables);
        }

        private ChelType ProcessParameter(ChelType input, VariableCollection variables)
        {
            switch(input)
            {
                case CompoundValue compoundValue:
                    return ProcessCompoundValue(compoundValue, variables);

                case Literal literal:
                    return literal;

                case VariableReference variableReference:
                    return ProcessVariableReference(variableReference, variables);

                case List listValue:
                    return ProcessList(listValue, variables);
            }

            throw new InvalidOperationException("Internal error: Unknown ChelType.");
        }

        private ChelType ProcessCompoundValue(CompoundValue input, VariableCollection variables)
        {
            var replacedValues = new List<ChelType>(input.Values.Count);

            foreach(var subvalue in input.Values)
            {
                var replaced = ProcessParameter(subvalue, variables);
                replacedValues.Add(replaced);
            }

            return new CompoundValue(replacedValues);
        }

        private ChelType ProcessList(List input, VariableCollection variables)
        {
            var replacedValues = new List<ChelType>(input.Values.Count);

            foreach(var subvalue in input.Values)
            {
                var replaced = ProcessParameter(subvalue, variables);
                replacedValues.Add(replaced);
            }

            return new List(replacedValues);
        }

        private ChelType ProcessVariableReference(VariableReference input, VariableCollection variables)
        {
            var variable = variables.Get(input.VariableName);

            if(variable == null)
                throw new UnsetVariableException(input.VariableName);

            return variable.Value;
        }
    }
}
