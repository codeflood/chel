namespace Chel.Abstractions.Results
{
    /// <summary>
    /// A <see cref="CommandResult" /> indicating the command was not known.
    /// </summary>
    public class UnknownCommandResult : FailureResult
    {
        public UnknownCommandResult(int sourceLine)
            : base(sourceLine)
        {
        }

        public override string ToString()
        {
            return $"{base.ToString()}: {Texts.UnknownCommand}";
        }
    }
}