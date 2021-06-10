using Chel.Abstractions.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chel.Parsing
{
    public class ParseWordState : ITokenizerState
    {
        private StringBuilder _buffer = new StringBuilder();

        private ITokenizerState _innerState = null;

        public IEnumerable<TokenizerStateResponse> Process(char input)
        {
            if(_innerState != null)
                return ProcessWithInnerState(input);

            if(input == Symbols.Escape)
            {
                _innerState = new ParseLiteralInnerState();
                return _innerState.Process(input);
            }

            if(char.IsWhiteSpace(input))
            {
                if(_buffer.Length == 0)
                {
                    return new TokenizerStateResponse[]
                    {
                        new SetStateResponse(SkipWhiteSpaceState.Instance, true)
                    };
                }

                return new TokenizerStateResponse[]
                {
                    new EmitResponse(new LiteralToken(_buffer.ToString())),
                    new SetStateResponse(SkipWhiteSpaceState.Instance, true)
                };
            }

            if(input == Symbols.Comment)
            {
                if(_buffer.Length == 0)
                {
                    return new TokenizerStateResponse[]
                    {
                        new SetStateResponse(SkipCommentState.Instance, false)
                    };
                }

                return new TokenizerStateResponse[]
                {
                    new EmitResponse(new LiteralToken(_buffer.ToString())),
                    new SetStateResponse(SkipCommentState.Instance, false)
                };
            }

            _buffer.Append(input);

            return new[] { ContinueResponse.Instance };
        }

        private IEnumerable<TokenizerStateResponse> ProcessWithInnerState(char input)
        {
            var result = _innerState.Process(input);
            var single = result.Single();
            
            switch(single)
            {
                case ContinueResponse _:
                    return new[]{ single };
                
                case EmitResponse emitResponse:
                    var literalToken = emitResponse.Token as LiteralToken
                        ?? throw new InvalidOperationException(Texts.ParseErrorExpectedLitealToken);

                    _buffer.Append(literalToken.Value);
                    _innerState = null;
                    return new[] { ContinueResponse.Instance };

                default:
                    throw new InvalidOperationException(Texts.ParseErorUnexpectedResponseFromInnerState);
            }
        }
    }
}
