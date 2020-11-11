using System;

namespace Chel.Abstractions.Parsing
{
    public class SetStateResponse : TokenizerStateResponse
    {
        /// <summary>
        /// Gets the next state to set.
        /// </sumamry>
        public ITokenizerState State { get; }

        /// <summary>
        /// Indicates whether the current input should be reprocessed with the next state.
        /// </summary>
        public bool Reprocess { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="state">The next state to set.</param>
        public SetStateResponse(ITokenizerState state, bool reprocess)
        {
            if(state == null)
                throw new ArgumentNullException(nameof(state));

            State = state;
            Reprocess = reprocess;
        }
    }
}
