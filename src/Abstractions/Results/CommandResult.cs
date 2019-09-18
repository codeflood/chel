namespace Chel.Abstractions.Results
{
    /// <summary>
    /// A result from executing a command.
    /// </summary>
    public abstract class CommandResult
    {
        /// <summary>
        /// Gets whether the command execution was successful or not.
        /// </summary>
        public bool Success { get; protected set;}
    }
}