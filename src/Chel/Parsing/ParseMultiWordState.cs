using Chel.Abstractions.Parsing;
using System.Collections.Generic;
using System.Text;

namespace Chel.Parsing
{
    public class ParseMultiWordState : ITokenizerState
    {
        private StringBuilder _buffer = new StringBuilder();

        private ITokenizerState _innerState = null;

        private int _openingParenthesisCount = 0;

        private bool _capturing = false;

        public IEnumerable<TokenizerStateResponse> Process(char input)
        {
            if(_buffer.Length == 0 && _openingParenthesisCount == 0 && input != Symbols.BlockStart)
            {
                return new TokenizerStateResponse[] {
                    new SetStateResponse(SkipWhiteSpaceState.Instance, true)
                };
            }

            if(input == Symbols.BlockStart)
                _openingParenthesisCount++;
            else if(input == Symbols.BlockEnd)
            {
                return new TokenizerStateResponse[] {
                    new EmitResponse(new LiteralToken(_buffer.ToString())),
                    new SetStateResponse(SkipWhiteSpaceState.Instance, true)
                };
            }
            else
                _buffer.Append(input);

            return new[]{ ContinueResponse.Instance };
        }
    }
}
