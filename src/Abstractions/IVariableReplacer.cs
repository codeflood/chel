using Chel.Abstractions.Parsing;
using Chel.Abstractions.Variables;

namespace Chel.Abstractions
{
    /// <summary>
    /// Replaces variables in input.
    /// </summary>
    internal interface IVariableReplacer
    {
        /// <summary>
        /// Replace variables in the provided input.
        /// </summary>
        /// <param name="variables">The variables available for replacing.</param>
        /// <param name="input">The input to replace the variables in.</param>
        /// <returns>A string with the variables replaced.</returns>
        ICommandParameter ReplaceVariables(VariableCollection variables, ICommandParameter input);
    }
}
