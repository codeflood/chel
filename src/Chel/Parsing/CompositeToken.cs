using System.Collections.Generic;

namespace Chel.Parsing
{
    internal class CompositeToken : Token
    {
        public IReadOnlyList<Token> Tokens { get; }

        public CompositeToken(IList<Token> tokens)
        {
            if(tokens == null)
                Tokens = new List<Token>();
            else
                Tokens = new List<Token>(tokens);
        }
    }
}
