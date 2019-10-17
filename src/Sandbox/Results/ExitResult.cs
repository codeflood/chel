using Chel.Abstractions.Results;

namespace Sandbox.Commands
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