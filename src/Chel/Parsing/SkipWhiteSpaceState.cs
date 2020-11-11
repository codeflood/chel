using Chel.Abstractions.Parsing;

namespace Chel.Parsing
{
    public class SkipWhiteSpaceState : ITokenizerState
    {
        public TokenizerStateResponse Process(char input)
        {
            if(input == '\n')
                return new EmitResponse(new EndOfBlockToken());

            if(char.IsWhiteSpace(input))
                return ContinueResponse.Instance;

            return new SetStateResponse(new ParseWordState(), true);
        }
    }
}
