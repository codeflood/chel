using System;

namespace Chel.Abstractions.Parsing
{
    public class EmitResponse : TokenizerStateResponse
    {
        /// <summary>
        /// Gets the token to emit.
        /// </summary>
        public Token Token { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="token">The token to emit.</param>
        public EmitResponse(Token token)
        {
            if(token == null)
                throw new ArgumentNullException(nameof(token));

            Token = token;
        }
    }
}
