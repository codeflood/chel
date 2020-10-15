using System;
using System.IO;
using System.Text;
using Chel.Abstractions;
using Chel.Abstractions.Variables;
using Chel.Exceptions;

namespace Chel
{
    internal class VariableReplacer : IVariableReplacer
    {
        public string ReplaceVariables(VariableCollection variables, string input)
        {
            if(variables == null)
                throw new ArgumentNullException(nameof(variables));

            if(input == null)
                throw new ArgumentNullException(nameof(input));

            var reader = new StringReader(input);

            var escaping = false;
            var capturingVariableName = false;
            var variableName = new StringBuilder();
            var output = new StringBuilder();

            var character = reader.Read();

            while(character != -1)
            {
                if(character == Symbols.Escape)
                    escaping = true;
                else if(character == Symbols.Variable && !escaping)
                {
                    if(capturingVariableName)
                    {
                        var capturedVariableName = variableName.ToString();
                        var variable = variables.Get(capturedVariableName);

                        if(variable == null)
                            throw new UnsetVariableException(capturedVariableName);

                        output.Append((variable as ValueVariable).Value);

                        variableName.Clear();
                        capturingVariableName = false;
                    }
                    else
                        capturingVariableName = true;
                }
                else if(capturingVariableName)
                    variableName.Append((char)character);
                else
                {
                    output.Append((char)character);

                    escaping = false;
                }

                character = reader.Read();
            }

            if(capturingVariableName)
                throw new ArgumentException(Texts.UnpairedVariableToken, nameof(input));

            return output.ToString();
        }
    }
}
