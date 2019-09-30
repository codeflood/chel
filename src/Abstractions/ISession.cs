using System;
using Chel.Abstractions.Results;

namespace Chel.Abstractions
{
    /// <summary>
    /// An isolated session in which commands are run.
    /// </summary>
    public interface ISession
    {
        /// <summary>
        /// Execute input.
        /// </summary>
        void Execute(string input, Action<CommandResult> resultHandler);
    }
}