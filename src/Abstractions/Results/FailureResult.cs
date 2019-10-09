namespace Chel.Abstractions.Results
{
    /// <summary>
    /// A failed <see cref="CommandResult" />.
    /// </summary>
    public class FailureResult : CommandResult
    {
        /// <summary>
        /// Gets the line on which the failure occurred.
        /// </summary>
        public int SourceLine { get; }

        /// <summary>
        /// Create a new instance.
        /// </sumamry>
        /// <param name="sourceLine">The line on which the failure occurred.</param>
        public FailureResult(int sourceLine)
        {
            Success = false;
            SourceLine = sourceLine;
        }

        public override string ToString()
        {
            return string.Format(Texts.ErrorOnLine, SourceLine);
        }
    }
}