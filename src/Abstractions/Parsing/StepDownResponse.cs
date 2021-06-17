using System;

namespace Chel.Abstractions.Parsing
{
    /// <summary>
    /// Makes the current tokenizer state inactive.
    /// </summary>
    public class StepDownResponse : TokenizerStateResponse
    {
        public static StepDownResponse Instance = new StepDownResponse();
    }
}