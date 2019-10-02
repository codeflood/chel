namespace Chel.Abstractions.Results
{
    /// <summary>
    /// A <see cref="CommandResult" /> indicating the command was not known.
    /// </summary>
    public class UnknownCommandResult : CommandResult
    {
        public UnknownCommandResult()
        {
            Success = false;
        }

        public override string ToString()
        {
            return "Unknown command";
        }
    }
}