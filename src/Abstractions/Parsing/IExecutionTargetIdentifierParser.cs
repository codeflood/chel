namespace Chel.Abstractions.Parsing
{
    /// <summary>
    /// Defines a parser to parse an <see cref="ExecutionTargetIdentifier"/> from user input.
    /// </summary>
    public interface IExecutionTargetIdentifierParser
    {
        /// <summary>
        /// Parse a <see cref="ExecutionTargetIdentifier"/> from the provided input.
        /// </summary>
        /// <param name="input">The input to parse.</param>
        /// <returns>The parsed <see cref="ExecutionTargetIdentifier"/>.</returns>
        ExecutionTargetIdentifier Parse(string input);
    }
}
