using System;
using System.IO;
using Chel.Abstractions.Parsing;

namespace Chel.Parsing
{
    /// <summary>
    /// The default implementation of the <see cref="ITokenizer" />.
    /// </summary>
    public class Tokenizer : ITokenizer
    {
        private StringReader _reader = null;
        private ITokenizerState _state = null;

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="input">The input to tokenize.</param>
        public Tokenizer(string input)
        {
            _reader = new StringReader(input ?? string.Empty);
            _state = new SkipWhiteSpaceState();
        }

        public Token GetNextToken()
        {
            var c = _reader.Read();

            while(c > 0)
            {
                var response = _state.Process((char)c);
                var reprocessChar = false;

                switch(response)
                {
                    case EmitResponse emitResponse:
                        return emitResponse.Token;

                    case SetStateResponse setStateResponse:
                        _state = setStateResponse.State;
                        reprocessChar = setStateResponse.Reprocess;
                        break;

                    case ContinueResponse _:
                        break;

                    default:
                        throw new Exception("unknown response from parser state: " + response.GetType().FullName);
                }

                if(!reprocessChar)
                    c = _reader.Read();
            }

            return null;
        }
    }
}
