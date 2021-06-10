using System.Collections.Generic;
using Chel.Abstractions.Parsing;

namespace Chel.Parsing
{
    public class SkipCommentState : ITokenizerState
    {
        public static SkipCommentState Instance { get; } = new SkipCommentState();

        public IEnumerable<TokenizerStateResponse> Process(char input)
        {
            if(input == '\n')
                return new[] { new EmitResponse(new EndOfBlockToken()) };

            return new[] { ContinueResponse.Instance };
        }
    }
}