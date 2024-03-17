namespace Chel.Abstractions.Parsing
{
    public class ExecutionTargetIdentifierParser : IExecutionTargetIdentifierParser
    {
        public ExecutionTargetIdentifier Parse(string input)
        {
            if(string.IsNullOrWhiteSpace(input))
                return new ExecutionTargetIdentifier(null, string.Empty);
            
            var index = input.IndexOf(Symbol.SubName);
            if(index < 0)
                return new ExecutionTargetIdentifier(null, input);

            var module = input.Substring(0, index);
            var command = input.Substring(index + 1);
            return new ExecutionTargetIdentifier(module, command);
        }
    }
}
