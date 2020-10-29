using System;

namespace Chel.Parsing
{
    internal class VariableToken : Token
    {
        public string VariableName { get; }

        public VariableToken(string variableName)
        {
            if(variableName == null)
                throw new ArgumentNullException(nameof(variableName));

            if(string.IsNullOrWhiteSpace(variableName))
                throw new ArgumentException(string.Format(Abstractions.Texts.ArgumentCannotBeEmptyOrWhitespace, nameof(variableName)), nameof(variableName));

            VariableName = variableName;
        }
    }
}