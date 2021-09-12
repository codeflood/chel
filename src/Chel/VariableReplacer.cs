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

            throw new InvalidOperationException(Texts.InternalErrorUnknownChelType);
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
            var variableName = input.VariableName;
            var subreferences = new Queue<string>(input.SubReferences);

            var variable = variables.Get(variableName);

            if(variable == null)
                throw new UnsetVariableException(input.VariableName);

            var value = variable.Value;

            do
            {
                string subreference = null;
                
                if(subreferences.Count > 0)
                    subreference = subreferences.Dequeue();

                if(value is List listValue)
                    value = ProcessListVariableReference(listValue, variableName, subreference);
                else if(subreference != null)
                    throw new InvalidOperationException(string.Format(Texts.VariableSubreferenceIsInvalid, variableName, subreference));
                
                if(subreferences.Count > 0)
                    variableName += Symbol.SubName + subreference;
            }
            while(subreferences.Count > 0);

            return value;
        }

        private ChelType ProcessListVariableReference(List listValue, string variableName, string subreference)
        {
            if(string.IsNullOrWhiteSpace(subreference))
                return listValue;

            var oneBasedIndex = 0;

            switch(subreference.ToUpper())
            {
                case "COUNT":
                    return new Literal(listValue.Values.Count.ToString());

                case "FIRST":
                    oneBasedIndex = 1;
                    break;

                case "LAST":
                    oneBasedIndex = listValue.Values.Count;
                    break;

                default:
                    if(!int.TryParse(subreference, out oneBasedIndex))
                        throw new InvalidOperationException(string.Format(Texts.VariableSubreferenceIsInvalid, variableName, subreference));
                    break;
            }

            if(oneBasedIndex < 0)
                oneBasedIndex = listValue.Values.Count + (oneBasedIndex + 1);

            if(oneBasedIndex < 1 || oneBasedIndex > listValue.Values.Count)
                throw new InvalidOperationException(string.Format(Texts.VariableSubreferenceIsInvalid, variableName, subreference));

            return listValue.Values[oneBasedIndex - 1];
        }
    }
}
