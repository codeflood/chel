using Chel.Abstractions.Results;

namespace Chel.Sandbox.Results
{
    public class ExitResult : CommandResult
    {
        public ExitResult()
        {
            Success = true;
        }

        public override string ToString() 
        {
            return string.Empty;
        }
    }
}