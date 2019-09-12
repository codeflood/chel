using System.Collections.Generic;

namespace Chel.Abstractions
{
    /// <summary>
    /// Parses command inputs from textual input.
    /// </summary>
    public interface IParser
    {
        /// <summary>
        /// Parse a list of command inputs.
        /// </summary>
        /// <returns>A list of <see cref="CommandInput"/>.</returns>
        IList<CommandInput> Parse(string input);
    }
}
