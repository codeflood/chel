using System;
using Chel.Abstractions.Parsing;

namespace Chel.Parsing
{
    internal class ParseBlock
    {
        public string Block { get; }

        public bool IsEndOfLine { get; }

        public SourceLocation LocationStart { get; }

        public ParseBlock(SourceLocation locationStart, string block, bool isEndOfLine = false)
        {
            LocationStart = locationStart ?? throw new ArgumentNullException(nameof(locationStart));
            Block = block;
            IsEndOfLine = isEndOfLine;
        }
    }
}