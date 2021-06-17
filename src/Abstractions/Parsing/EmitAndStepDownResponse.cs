using System;

namespace Chel.Abstractions.Parsing
{
    public class EmitAndStepDownResponse : TokenizerStateResponse
    {
        /// <summary>
        /// Gets the token to emit.
        /// </summary>
        public Token Token { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="token">The token to emit.</param>
        public EmitAndStepDownResponse(Token token)
        {
            if(token == null)
                throw new ArgumentNullException(nameof(token));

            Token = token;
        }
    }
}
