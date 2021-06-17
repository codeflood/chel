using System;
using System.Collections.Generic;
using System.IO;
using Chel.Abstractions.Exceptions;
using Chel.Abstractions.Parsing;

namespace Chel.Parsing
{
    /// <summary>
    /// The default implementation of the <see cref="ITokenizer" />.
    /// </summary>
    public class Tokenizer : ITokenizer
    {
        private StringReader _reader = null;
        private List<ITokenizerState> _states = null;
        private ITokenizerState _activeState = null;

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="input">The input to tokenize.</param>
        public Tokenizer(string input)
        {
            // Add a newline to the input to ensure the last state flushes properly.
            _reader = new StringReader((input ?? string.Empty) + "\n");
            _states = new List<ITokenizerState>
            {
                new SkipWhiteSpaceState(),
                new SkipCommentState(),
                new ParseWordState()
            };
        }

        public Token GetNextToken()
        {
            var c = _reader.Read();

            bool reprocess = false;

            while(c > 0)
            {
                var input = (char)c;
                reprocess = false;

                if(_activeState == null)
                    SetActiveState(input);

                /*if(_activeState == null)
                    // todo: The exception should include the exact location of the error, line and column.
                    throw new ParsingException("Unable to parse input. Unexpected input " + c);*/

                var response = _activeState.Process(input);
                
                switch(response)
                {
                    case StepDownResponse _:
                        _activeState = null;
                        /*SetActiveState(input);

                        if(_activeState == null)
                            // todo: The exception should include the exact location of the error, line and column.
                            throw new ParsingException("Unable to parse input. Unexpected input " + c);*/

                        reprocess = true;

                        break;

                    case EmitAndStepDownResponse emitResponse:
                        _activeState = null;
                        reprocess = true;
                        return emitResponse.Token;

                    /*case SetStateResponse setStateResponse:
                        _state = setStateResponse.State;
                        reprocessChar = setStateResponse.Reprocess;
                        break;*/

                    case ContinueResponse _:
                        break;

                    default:
                        throw new Exception("unknown response from parser state: " + response.GetType().FullName);
                }

                if(!reprocess)
                    c = _reader.Read();
            }

            return null;
        }

        private void SetActiveState(char input)
        {
            foreach(var state in _states)
            {
                if(state.CanProcess(input))
                {
                    _activeState = state;
                    return;
                }
            }

            //_activeState = null;
            // todo: The exception should include the exact location of the error, line and column.
            throw new ParsingException("Unable to parse input. Unexpected input " + input);
        }
    }
}
