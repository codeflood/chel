using System.Collections.Generic;
using Chel.Abstractions.Parsing;

namespace Chel.Parsing
{
    public class SkipWhiteSpaceState : ITokenizerState
    {
		public bool CanProcess(char input)
		{
			return char.IsWhiteSpace(input);
		}

        public TokenizerStateResponse Process(char input)
        {
            if(input == '\n')
                return new EmitAndStepDownResponse(EndOfBlockToken.Instance);

            if(char.IsWhiteSpace(input))
                return ContinueResponse.Instance;

            return StepDownResponse.Instance;
        }
	}
}
