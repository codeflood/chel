using Chel.Abstractions.Parsing;
using System.Text;

namespace Chel.Parsing
{
    public class ParseWordState : ITokenizerState
    {
        private StringBuilder _buffer = new StringBuilder();

        private int _parenthesisCount = 0;

        // todo: Ignore comments inside brackets.
        // todo: Escaping characters, like brakcets. \(

        public bool CanProcess(char input)
		{
			return !char.IsWhiteSpace(input);
		}

        public TokenizerStateResponse Process(char input)
        {
            if(input == Symbols.BlockEnd)
            {
                _parenthesisCount--;

                if(_parenthesisCount == 0)
                {
                    if(_buffer.Length > 0)
                    {
                        var token = new LiteralToken(_buffer.ToString());
                    _buffer.Clear();
                    return new EmitAndStepDownResponse(token);
                    }

                    return StepDownResponse.Instance;
                }
            }

            if(_parenthesisCount == 0 && char.IsWhiteSpace(input))
            {
                if(_buffer.Length > 0)
                {
                    var token = new LiteralToken(_buffer.ToString());
                    _buffer.Clear();
                    return new EmitAndStepDownResponse(token);
                }

                return StepDownResponse.Instance;
            }

            if(_parenthesisCount == 0 && input == Symbols.Comment)
            {
                if(_buffer.Length > 0)
                {
                    var token = new LiteralToken(_buffer.ToString());
                    _buffer.Clear();
                    return new EmitAndStepDownResponse(token);
                }

                return StepDownResponse.Instance;
            }

            if(input == Symbols.BlockStart)
            {
                _parenthesisCount++;

                if(_parenthesisCount > 1)
                    _buffer.Append(input);
            }
            else
                _buffer.Append(input);

            return ContinueResponse.Instance;
        }
    }
}
