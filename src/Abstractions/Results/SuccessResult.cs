namespace Chel.Abstractions.Results
{
    /// <summary>
    /// A successful <see cref="CommandResult"/> with no return value.
    /// </summary>
    public class SuccessResult : CommandResult
    {
        public SuccessResult()
        {
            Success = true;
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}