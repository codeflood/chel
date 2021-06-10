using System.Collections.Generic;

namespace Chel.Abstractions.Parsing
{
    /// <summary>
    /// An internal state of the <see cref="ITokenizer"/>.
    /// </summary>
    public interface ITokenizerState
    {
        IEnumerable<TokenizerStateResponse> Process(char input);
    }
}
