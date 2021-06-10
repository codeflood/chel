using System.Collections.Generic;
using Chel.Abstractions.Parsing;

namespace Chel.Parsing
{
    public class SkipWhiteSpaceState : ITokenizerState
    {
        public static SkipWhiteSpaceState Instance { get; } = new SkipWhiteSpaceState();

        public IEnumerable<TokenizerStateResponse> Process(char input)
        {
            if(input == '\n')
                return new[] { new EmitResponse(new EndOfBlockToken()) };

            if(char.IsWhiteSpace(input))
                return new[] { ContinueResponse.Instance };

            return new[] { new SetStateResponse(new ParseWordState(), true) };
        }
    }
}
