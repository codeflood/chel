using System;
using Chel.Abstractions.Parsing;

namespace Chel.Parsing
{
    internal class ParseBlock
    {
        public CommandParameter Block { get; }

        public bool IsEndOfLine { get; }

        public SourceLocation LocationStart { get; }

        public ParseBlock(SourceLocation locationStart, CommandParameter block, bool isEndOfLine = false)
        {
            LocationStart = locationStart ?? throw new ArgumentNullException(nameof(locationStart));
            Block = block;
            IsEndOfLine = isEndOfLine;
        }
    }
}