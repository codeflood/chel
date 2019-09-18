namespace Chel.Abstractions.Results
{
    /// <summary>
    /// A successful <see cref="CommandResult"/> with no return value.
    /// </summary>
    public class Success : CommandResult
    {
        public Success()
        {
            Success = true;
        }
    }
}