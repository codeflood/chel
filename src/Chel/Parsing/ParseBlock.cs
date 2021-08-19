using System;
using Chel.Abstractions.Parsing;
using Chel.Abstractions.Types;

namespace Chel.Parsing
{
    internal class ParseBlock
    {
        public ChelType Block { get; }

        public bool IsEndOfLine { get; }

        public SourceLocation LocationStart { get; }

        public ParseBlock(SourceLocation locationStart, ChelType block, bool isEndOfLine = false)
        {
            LocationStart = locationStart ?? throw new ArgumentNullException(nameof(locationStart));
            Block = block;
            IsEndOfLine = isEndOfLine;
        }
    }
}