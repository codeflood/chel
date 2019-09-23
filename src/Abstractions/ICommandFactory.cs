namespace Chel.Abstractions
{
    /// <summary>
    /// Creates instances of commands.
    /// </summary>
    public interface ICommandFactory
    {
        /// <summary>
        /// Create a command from the input.
        /// </summary>
        ICommand Create(CommandInput commandInput);
    }
}