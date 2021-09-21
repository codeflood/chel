using System;
using Chel.Abstractions.Parsing;
using Chel.Abstractions.Types;

namespace Chel.Parsing
{
    internal class ParseBlock
    {
        public ICommandParameter Block { get; }

        public bool IsEndOfLine { get; }

        public SourceLocation LocationStart { get; }

        public ParseBlock(SourceLocation locationStart, ICommandParameter block, bool isEndOfLine = false)
        {
            LocationStart = locationStart ?? throw new ArgumentNullException(nameof(locationStart));
            Block = block;
            IsEndOfLine = isEndOfLine;
        }
    }
}