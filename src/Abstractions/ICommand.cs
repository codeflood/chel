using Chel.Abstractions.Results;

namespace Chel.Abstractions
{
    /// <summary>
    /// A command which can be executed.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        CommandResult Execute();
    }
}
