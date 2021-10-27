using Chel.Abstractions.Parsing;

namespace Chel.Parsing
{
    internal class ParseBlock
    {
        public ICommandParameter Block { get; }

        public bool IsEndOfLine { get; }

        public SourceLocation LocationStart { get; }

        public ParseBlock(SourceLocation locationStart, ICommandParameter block, bool isEndOfLine = false)
        {
            LocationStart = locationStart;
            Block = block;
            IsEndOfLine = isEndOfLine;
        }
    }
}