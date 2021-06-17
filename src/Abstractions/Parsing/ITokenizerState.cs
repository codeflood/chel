using System.Collections.Generic;

namespace Chel.Abstractions.Parsing
{
    /// <summary>
    /// An internal state of the <see cref="ITokenizer"/>.
    /// </summary>
    public interface ITokenizerState
    {
        /// <summary>
        /// Indicates whether the state can process the input.
        /// </summary>
        bool CanProcess(char input);

        /// <summary>
        /// Process the input.
        /// </summary>
        TokenizerStateResponse Process(char input);
    }
}
