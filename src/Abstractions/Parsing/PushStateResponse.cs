using System;

namespace Chel.Abstractions.Parsing
{
    /// <summary>
    /// Push the current tokenizer state to the state stack and set the new state.
    /// </summary>
    public class PushStateResponse : TokenizerStateResponse
    {
        /// <summary>
        /// Gets the next state to set.
        /// </sumamry>
        public ITokenizerState State { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="state">The next state to set.</param>
        public PushStateResponse(ITokenizerState state)
        {
            if(state == null)
                throw new ArgumentNullException(nameof(state));

            State = state;
        }
    }
}