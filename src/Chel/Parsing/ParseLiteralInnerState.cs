using System.Collections.Generic;
using Chel.Abstractions.Parsing;

namespace Chel.Parsing
{
    public class ParseLiteralInnerState : ITokenizerState
    {
        private bool _escaped = false;

        public IEnumerable<TokenizerStateResponse> Process(char input)
        {
            if(_escaped && !char.IsWhiteSpace(input))
            {
                _escaped = false;
                return new[] { new EmitResponse(new LiteralToken(input.ToString())) };
            }

            if(input == Symbols.Escape)
            {
                _escaped = true;
                return new[] { ContinueResponse.Instance };
            }

            return new[] { new EmitResponse(new LiteralToken(string.Empty)) };
        }
    }
}
