namespace Chel.Abstractions.Results
{
    /// <summary>
    /// A <see cref="CommandResult" /> indicating the command was not known.
    /// </summary>
    public class UnknownCommand : CommandResult
    {
        public UnknownCommand()
        {
            Success = false;
        }
    }
}