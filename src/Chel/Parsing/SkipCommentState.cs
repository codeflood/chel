using Chel.Abstractions.Parsing;

namespace Chel.Parsing
{
	public class SkipCommentState : ITokenizerState
    {
        public bool CanProcess(char input)
		{
			return input == Symbols.Comment;
		}

        public TokenizerStateResponse Process(char input)
        {
            if(input == '\n')
                return StepDownResponse.Instance;

            return ContinueResponse.Instance;
        }
    }
}